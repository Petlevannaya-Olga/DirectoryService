using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using DirectoryService.Application;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Primitives;

namespace DirectoryService.Infrastructure;

public class PositionsRepository(
    ApplicationDbContext dbContext,
    ILogger<PositionsRepository> logger) : IPositionsRepository
{
    public async Task<Result<Guid, Error>> AddAsync(Position position, CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.Positions.AddAsync(position, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException e) when (e.InnerException is PostgresException postgresException)
        {
            if (postgresException is { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: not null }
                && postgresException.ConstraintName.Contains(
                    "positions_name",
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return Errors.PositionNameConflict(position.Name.Value);
            }

            logger.LogError(e, "Ошибка добавления новой позиции '{PositionName}'", position.Name.Value);
            return CommonErrors.Db(
                "add.position.to.db.exception",
                $"Ошибка добавления новой позиции '{position.Name.Value}'");
        }
        catch (OperationCanceledException e)
        {
            logger.LogError(e, "Операция создания новой позиции '{Position}' была отменена", position.Name.Value);
            return CommonErrors.OperationCancelled("add.position.was.canceled");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка добавления позиции '{PositionName}'", position.Name.Value);
            return CommonErrors.Db(
                "add.position.to.db.exception",
                $"Ошибка добавления новой позиции '{position.Name.Value}'");
        }

        return position.Id.Value;
    }

    public async Task<Result<Position?, Error>> GetByAsync(
        Expression<Func<Position, bool>> expression,
        CancellationToken cancellationToken)
    {
        try
        {
            return await dbContext
                .Positions
                .FirstOrDefaultAsync(expression, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogError("Операция получения позиции была отменена");
            return CommonErrors.OperationCancelled("get.position.was.canceled");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении позиции");
            return CommonErrors.Db(
                "get.position.from.db.exception",
                $"Ошибка при получении позиции");
        }
    }

    [ExcludeFromCodeCoverage]
    private static class Errors
    {
        public static Error PositionNameConflict(string positionName)
        {
            return CommonErrors.Conflict(
                "position.name.conflict",
                $"Позиция с заголовком {positionName} уже существует");
        }
    }
}