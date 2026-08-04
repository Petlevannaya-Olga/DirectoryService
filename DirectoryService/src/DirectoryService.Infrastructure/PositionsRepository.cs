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

public sealed class PositionsRepository(
    ApplicationDbContext dbContext,
    ILogger<PositionsRepository> logger)
    : IPositionsRepository
{
    public async Task<Result<Guid, Error>> AddAsync(
        Position position,
        CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.Positions.AddAsync(
                position,
                cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);

            return position.Id.Value;
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException postgresException)
        {
            if (postgresException is
                {
                    SqlState: PostgresErrorCodes.UniqueViolation,
                    ConstraintName: not null
                }
                && postgresException.ConstraintName.Contains(
                    "positions_name",
                    StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning(
                    "Позиция с названием '{PositionName}' уже существует",
                    position.Name.Value);

                return Errors.PositionNameConflict(
                    position.Name.Value);
            }

            logger.LogError(
                exception,
                "Ошибка добавления позиции '{PositionName}'",
                position.Name.Value);

            return CommonErrors.Db(
                "add.position.to.db.exception",
                $"Ошибка добавления позиции '{position.Name.Value}'");
        }
        catch (OperationCanceledException exception)
        {
            logger.LogWarning(
                exception,
                "Операция создания позиции '{PositionName}' была отменена",
                position.Name.Value);

            return CommonErrors.OperationCancelled(
                "add.position.was.canceled");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Ошибка добавления позиции '{PositionName}'",
                position.Name.Value);

            return CommonErrors.Db(
                "add.position.to.db.exception",
                $"Ошибка добавления позиции '{position.Name.Value}'");
        }
    }

    public async Task<Result<bool, Error>> ExistsAsync(
        Expression<Func<Position, bool>> expression,
        CancellationToken cancellationToken)
    {
        try
        {
            var exists = await dbContext
                .Positions
                .AnyAsync(
                    expression,
                    cancellationToken);

            return exists;
        }
        catch (OperationCanceledException exception)
        {
            logger.LogWarning(
                exception,
                "Операция проверки существования позиции была отменена");

            return CommonErrors.OperationCancelled(
                "check.position.exists.was.canceled");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Ошибка проверки существования позиции");

            return CommonErrors.Db(
                "check.position.exists.in.db.exception",
                "Ошибка проверки существования позиции");
        }
    }

    public async Task<Result<Position, Error>> GetByAsync(
        Expression<Func<Position, bool>> expression,
        CancellationToken cancellationToken)
    {
        try
        {
            var position = await dbContext
                .Positions
                .FirstOrDefaultAsync(
                    expression,
                    cancellationToken);

            if (position is null)
            {
                return CommonErrors.NotFound(
                    "position.not.found",
                    "Позиция не найдена");
            }

            return position;
        }
        catch (OperationCanceledException exception)
        {
            logger.LogWarning(
                exception,
                "Операция получения позиции была отменена");

            return CommonErrors.OperationCancelled(
                "get.position.was.canceled");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Ошибка получения позиции");

            return CommonErrors.Db(
                "get.position.from.db.exception",
                "Ошибка получения позиции");
        }
    }

    [ExcludeFromCodeCoverage]
    private static class Errors
    {
        public static Error PositionNameConflict(
            string positionName)
        {
            return CommonErrors.Conflict(
                "position.name.conflict",
                $"Позиция с названием '{positionName}' уже существует");
        }
    }
}