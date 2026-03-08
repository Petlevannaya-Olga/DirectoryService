using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using DirectoryService.Application;
using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Primitives;

namespace DirectoryService.Infrastructure;

public class DepartmentsRepository(
    ApplicationDbContext dbContext,
    ILogger<DepartmentsRepository> logger) : IDepartmentsRepository
{
    public async Task<Result<Guid, Error>> AddAsync(Department department, CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.Departments.AddAsync(department, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return department.Id.Value;
        }
        catch (DbUpdateException e) when (e.InnerException is PostgresException postgresException)
        {
            if (postgresException is { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: not null })
            {
                if (postgresException.ConstraintName.Contains(
                        "departments_name",
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    logger.LogError(
                        "Подразделение с названием '{DepartmentName}' уже существует",
                        department.Name.Value);
                    return Errors.DepartmentsNameConflict(department.Name.Value);
                }

                if (postgresException.ConstraintName.Contains(
                        "departments_identifier",
                        StringComparison.InvariantCultureIgnoreCase))
                {
                    logger.LogError(
                        "Подразделение с идентификатором '{DepartmentIdentifier}' уже существует",
                        department.Identifier.Value);
                    return Errors.DepartmentsIdentifierConflict(department.Identifier.Value);
                }
            }

            logger.LogError(e, "Ошибка добавления нового подразделения '{PositionName}'", department.Name.Value);

            return CommonErrors.Db(
                "add.department.to.db.exception",
                $"Ошибка добавления нового подразделения '{department.Name.Value}'");
        }
        catch (OperationCanceledException e)
        {
            logger.LogError(
                e,
                "Операция добавления нового подразделения '{DepartmentName}' была отменена",
                department.Name.Value);

            return CommonErrors.OperationCancelled("add.department.was.canceled");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка добавления нового подразделения '{DepartmentName}'", department.Name.Value);
            return CommonErrors.Db(
                "add.department.to.db.exception",
                $"Ошибка добавления нового подразделения '{department.Name.Value}'");
        }
    }

    public async Task<Result<Department?, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return await dbContext
                .Departments
                .FirstOrDefaultAsync(x => x.Id == new DepartmentId(id), cancellationToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogError("Операция получения подразделения была отменена");
            return CommonErrors.OperationCancelled("get.department.was.canceled");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении подразделения c id = {Id}", id);
            return CommonErrors.Db(
                "get.position.from.db.exception",
                $"Ошибка при получении подразделения c id '{id}'");
        }
    }

    public async Task<Result<bool, Error>> ExistsAndActive(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    {
        try
        {
            var departmentIds = ids.Select(x => new DepartmentId(x)).ToList();

            int existingCount = await dbContext
                .Departments
                .CountAsync(d => departmentIds.Contains(d.Id) && d.IsActive, cancellationToken);

            if (existingCount == departmentIds.Count)
            {
                return true;
            }

            logger.LogError("Некоторые подразделения не являются активными или отсутствуют в БД");
            return Errors.ActiveDepartmentsNotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Произошла непредвиденная ошибка в процессе проверки активных подразделений в БД");

            return Errors.UnexpectedDbException();
        }
    }

    [ExcludeFromCodeCoverage]
    private static class Errors
    {
        public static Error DepartmentsNameConflict(string departmentName)
        {
            return CommonErrors.Conflict(
                "department.name.conflict",
                $"Подразделение с заголовком {departmentName} уже существует");
        }

        public static Error DepartmentsIdentifierConflict(string identifier)
        {
            return CommonErrors.Conflict(
                "department.identifier.conflict",
                $"Подразделение с идентификатором {identifier} уже существует");
        }

        public static Error ActiveDepartmentsNotFound()
        {
            return CommonErrors.NotFound(
                "active.positions.not_found",
                "Некоторые подразделения не являются активными или отсутствуют в БД");
        }

        public static Error UnexpectedDbException()
        {
            return CommonErrors.Db(
                "get.exists.and.active.from.db.exception",
                $"Произошла непредвиденная ошибка в процессе проверки активных подразделений в БД");
        }
    }
}