using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Primitives;

namespace DirectoryService.Infrastructure.Database;

public sealed class TransactionManager(
    ApplicationDbContext dbContext,
    ILogger<TransactionManager> logger,
    ILogger<TransactionScope> transactionScopeLogger)
    : ITransactionManager
{
    public async Task<Result<ITransactionScope, Error>> BeginTransactionAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            IDbContextTransaction transaction =
                await dbContext.Database.BeginTransactionAsync(cancellationToken);

            ITransactionScope transactionScope =
                new TransactionScope(transaction, transactionScopeLogger);

            return Result.Success<ITransactionScope, Error>(transactionScope);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation("Создание транзакции было отменено");

            return CommonErrors.OperationCancelled(
                "begin.transaction.operation.cancelled");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Не удалось создать транзакцию");

            return CommonErrors.Failure(
                "db.transaction.begin.failed",
                "Не удалось создать транзакцию");
        }
    }

    public async Task<UnitResult<Error>> SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);

            return UnitResult.Success<Error>();
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation("Сохранение изменений было отменено");

            return UnitResult.Failure(
                CommonErrors.OperationCancelled(
                    "save.changes.operation.cancelled"));
        }
        catch (DbUpdateConcurrencyException exception)
        {
            logger.LogWarning(
                exception,
                "При сохранении произошёл конфликт конкурентного изменения");

            return UnitResult.Failure(
                CommonErrors.Conflict(
                    "db.concurrency.conflict",
                    "Данные были изменены другим пользователем"));
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(
                exception,
                "Ошибка базы данных при сохранении изменений");

            return UnitResult.Failure(
                CommonErrors.Failure(
                    "db.update.failed",
                    "Не удалось сохранить изменения в базе данных"));
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Не удалось сохранить изменения в базе данных");

            return UnitResult.Failure(
                CommonErrors.Failure(
                    "db.save.changes.failed",
                    "Не удалось сохранить изменения в базе данных"));
        }
    }
}