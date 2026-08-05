using System.Linq.Expressions;
using CSharpFunctionalExtensions;
using DirectoryService.Application;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Primitives;

namespace DirectoryService.Infrastructure;

public sealed class PositionsRepository(
    ApplicationDbContext dbContext,
    ILogger<PositionsRepository> logger)
    : IPositionsRepository
{
    public Task<Result<Guid, Error>> AddAsync(
        Position position,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult<Result<Guid, Error>>(
                CommonErrors.OperationCancelled(
                    "add.position.was.canceled"));
        }

        dbContext.Positions.Add(position);

        return Task.FromResult<Result<Guid, Error>>(
            position.Id.Value);
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
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation(
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
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation(
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
}