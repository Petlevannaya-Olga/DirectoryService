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
        }
        catch (DbUpdateException e) when (e.InnerException is PostgresException postgresException)
        {
            if (postgresException is { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: not null }
                && postgresException.ConstraintName.Contains(
                    "departments_name",
                    StringComparison.InvariantCultureIgnoreCase))
            {
                logger.LogError("Подразделение с названием '{DepartmentName}' уже существует", department.Name.Value);
                return Errors.DepartmentsNameConflict(department.Name.Value);
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

        return department.Id.Value;
    }

    public async Task<Result<Department?, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            return await dbContext
                .Departments
                .FirstOrDefaultAsync(x => x.Id.Value == id, cancellationToken);
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
            int existingCount = await dbContext
                .Departments
                .CountAsync(l => ids.Contains(l.Id.Value) && l.IsActive, cancellationToken);

            if (existingCount == ids.Count())
            {
                return true;
            }

            logger.LogError("Некоторые позиции не являются активными или отсутствуют в БД");
            return Errors.ActivePositionsNotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Произошла непредвиденная ошибка в процессе проверки активных позиций в БД");

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

        public static Error ActivePositionsNotFound()
        {
            return CommonErrors.NotFound(
                "active.positions.not_found",
                "Некоторые позиции не являются активными или отсутствуют в БД");
        }

        public static Error UnexpectedDbException()
        {
            return CommonErrors.Db(
                "get.exists.and.active.from.db.exception",
                $"Произошла непредвиденная ошибка в процессе проверки активных позиций в БД");
        }
    }
}