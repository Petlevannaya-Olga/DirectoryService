using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using DirectoryService.Application;
using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Primitives;

namespace DirectoryService.Infrastructure;

public sealed class DepartmentsRepository(
    ApplicationDbContext dbContext,
    ILogger<DepartmentsRepository> logger)
    : IDepartmentsRepository
{
    public async Task<Result<Guid, Error>> AddAsync(
        Department department,
        CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.Departments.AddAsync(
                department,
                cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return department.Id.Value;
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException postgresException)
        {
            if (postgresException is
                {
                    SqlState: PostgresErrorCodes.UniqueViolation,
                    ConstraintName: not null
                })
            {
                if (postgresException.ConstraintName.Contains(
                        "departments_name",
                        StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogWarning(
                        "Подразделение с названием '{DepartmentName}' уже существует",
                        department.Name.Value);

                    return Errors.DepartmentNameConflict(
                        department.Name.Value);
                }

                if (postgresException.ConstraintName.Contains(
                        "departments_slug",
                        StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogWarning(
                        "Подразделение с идентификатором '{DepartmentSlug}' уже существует",
                        department.Slug.Value);

                    return Errors.DepartmentSlugConflict(
                        department.Slug.Value);
                }
            }

            logger.LogError(
                exception,
                "Ошибка добавления подразделения '{DepartmentName}'",
                department.Name.Value);

            return CommonErrors.Db(
                "add.department.to.db.exception",
                $"Ошибка добавления подразделения '{department.Name.Value}'");
        }
        catch (OperationCanceledException exception)
        {
            logger.LogWarning(
                exception,
                "Операция добавления подразделения '{DepartmentName}' была отменена",
                department.Name.Value);

            return CommonErrors.OperationCancelled(
                "add.department.was.canceled");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Ошибка добавления подразделения '{DepartmentName}'",
                department.Name.Value);

            return CommonErrors.Db(
                "add.department.to.db.exception",
                $"Ошибка добавления подразделения '{department.Name.Value}'");
        }
    }

    public async Task<Result<Department, Error>> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var departmentId = new DepartmentId(id);

            var department = await dbContext
                .Departments
                .FirstOrDefaultAsync(
                    item => item.Id == departmentId,
                    cancellationToken);

            if (department is null)
            {
                return CommonErrors.NotFound(
                    "department.not.found",
                    $"Подразделение с идентификатором '{id}' не найдено",
                    id);
            }

            return department;
        }
        catch (OperationCanceledException exception)
        {
            logger.LogWarning(
                exception,
                "Операция получения подразделения с id = {DepartmentId} была отменена",
                id);

            return CommonErrors.OperationCancelled(
                "get.department.was.canceled");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Ошибка получения подразделения с id = {DepartmentId}",
                id);

            return CommonErrors.Db(
                "get.department.from.db.exception",
                $"Ошибка получения подразделения с идентификатором '{id}'");
        }
    }

    public async Task<Result<Department, Error>> GetByIdWithLocationsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var departmentId = new DepartmentId(id);

            var department = await dbContext
                .Departments
                .Include(item => item.DepartmentLocations)
                .FirstOrDefaultAsync(
                    item => item.Id == departmentId,
                    cancellationToken);

            if (department is null)
            {
                return CommonErrors.NotFound(
                    "department.not.found",
                    $"Подразделение с идентификатором '{id}' не найдено",
                    id);
            }

            return department;
        }
        catch (OperationCanceledException exception)
        {
            logger.LogWarning(
                exception,
                "Операция получения подразделения с id = {DepartmentId} была отменена",
                id);

            return CommonErrors.OperationCancelled(
                "get.department.with.locations.was.canceled");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Ошибка получения подразделения с id = {DepartmentId}",
                id);

            return CommonErrors.Db(
                "get.department.with.locations.from.db.exception",
                $"Ошибка получения подразделения с идентификатором '{id}'");
        }
    }

    public async Task<Result<bool, Error>> ExistsAndActive(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        try
        {
            var departmentIds = ids
                .Distinct()
                .Select(id => new DepartmentId(id))
                .ToList();

            var existingCount = await dbContext
                .Departments
                .CountAsync(
                    department =>
                        departmentIds.Contains(department.Id) &&
                        department.IsActive,
                    cancellationToken);

            if (existingCount == departmentIds.Count)
            {
                return true;
            }

            logger.LogWarning(
                "Некоторые подразделения отсутствуют или неактивны");

            return Errors.ActiveDepartmentsNotFound();
        }
        catch (OperationCanceledException exception)
        {
            logger.LogWarning(
                exception,
                "Операция проверки активных подразделений была отменена");

            return CommonErrors.OperationCancelled(
                "check.departments.exists.and.active.was.canceled");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Ошибка проверки активных подразделений");

            return Errors.UnexpectedDbException();
        }
    }

    [ExcludeFromCodeCoverage]
    private static class Errors
    {
        public static Error DepartmentNameConflict(
            string departmentName)
        {
            return CommonErrors.Conflict(
                "department.name.conflict",
                $"Подразделение с названием '{departmentName}' уже существует");
        }

        public static Error DepartmentSlugConflict(string slug)
        {
            return CommonErrors.Conflict(
                "department.slug.conflict",
                $"Подразделение с идентификатором '{slug}' уже существует");
        }

        public static Error ActiveDepartmentsNotFound()
        {
            return CommonErrors.NotFound(
                "active.departments.not.found",
                "Некоторые подразделения отсутствуют или неактивны");
        }

        public static Error UnexpectedDbException()
        {
            return CommonErrors.Db(
                "check.departments.exists.and.active.db.exception",
                "Ошибка проверки активных подразделений");
        }
    }
}