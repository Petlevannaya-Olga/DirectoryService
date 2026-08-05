using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using DirectoryService.Application;
using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Primitives;

namespace DirectoryService.Infrastructure;

public sealed class DepartmentsRepository(
    ApplicationDbContext dbContext,
    ILogger<DepartmentsRepository> logger)
    : IDepartmentsRepository
{
    public Task<Result<Guid, Error>> AddAsync(
        Department department,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult<Result<Guid, Error>>(
                CommonErrors.OperationCancelled(
                    "add.department.was.cancelled"));
        }

        dbContext.Departments.Add(department);

        return Task.FromResult<Result<Guid, Error>>(
            department.Id.Value);
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
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation(
                exception,
                "Получение подразделения {DepartmentId} было отменено",
                id);

            return CommonErrors.OperationCancelled(
                "get.department.was.cancelled");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Ошибка получения подразделения {DepartmentId}",
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
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation(
                exception,
                "Получение подразделения {DepartmentId} с локациями было отменено",
                id);

            return CommonErrors.OperationCancelled(
                "get.department.with.locations.was.cancelled");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Ошибка получения подразделения {DepartmentId} с локациями",
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
                .ToArray();

            var existingCount = await dbContext
                .Departments
                .CountAsync(
                    department =>
                        department.IsActive &&
                        departmentIds.Contains(department.Id),
                    cancellationToken);

            if (existingCount == departmentIds.Length)
            {
                return true;
            }

            logger.LogWarning(
                "Некоторые подразделения отсутствуют или неактивны");

            return Errors.ActiveDepartmentsNotFound();
        }
        catch (OperationCanceledException exception)
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation(
                exception,
                "Проверка активных подразделений была отменена");

            return CommonErrors.OperationCancelled(
                "check.departments.exists.and.active.was.cancelled");
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