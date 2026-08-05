using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Npgsql;
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
        catch (OperationCanceledException exception)
            when (cancellationToken.IsCancellationRequested)
        {
            logger.LogInformation(
                exception,
                "Сохранение изменений было отменено");

            return UnitResult.Failure(
                CommonErrors.OperationCancelled(
                    "save.changes.operation.cancelled"));
        }
        catch (DbUpdateConcurrencyException exception)
        {
            logger.LogWarning(
                exception,
                "Возник конфликт конкурентного изменения данных");

            return UnitResult.Failure(
                CommonErrors.Conflict(
                    "db.concurrency.conflict",
                    "Данные были изменены другим пользователем"));
        }
        catch (DbUpdateException exception)
            when (PostgresErrorMapper.TryMap(
                exception,
                out var mappedError))
        {
            logger.LogWarning(
                exception,
                "Нарушено ограничение базы данных "
                + "{ConstraintName}. Код ошибки: {ErrorCode}",
                GetConstraintName(exception),
                mappedError.Code);

            return UnitResult.Failure(mappedError);
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(
                exception,
                "Ошибка базы данных при сохранении изменений");

            return UnitResult.Failure(
                CommonErrors.Db(
                    "db.update.failed",
                    "Не удалось сохранить изменения в базе данных"));
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Не удалось сохранить изменения в базе данных");

            return UnitResult.Failure(
                CommonErrors.Db(
                    "db.save.changes.failed",
                    "Не удалось сохранить изменения в базе данных"));
        }
    }

    private static string? GetConstraintName(
        DbUpdateException exception)
    {
        return exception.InnerException is PostgresException postgresException
            ? postgresException.ConstraintName
            : null;
    }
}