using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Positions;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Positions.DeletePosition;

public sealed class DeletePositionHandler(
    IPositionsRepository positionsRepository,
    ITransactionManager transactionManager,
    ILogger<DeletePositionHandler> logger)
    : ICommandHandler<DeletePositionCommand>
{
    public async Task<UnitResult<Errors>> Handle(
        DeletePositionCommand command,
        CancellationToken cancellationToken)
    {
        var positionId =
            new PositionId(command.PositionId);

        var positionResult =
            await positionsRepository.GetByAsync(
                position => position.Id == positionId,
                cancellationToken);

        if (positionResult.IsFailure)
        {
            return positionResult.Error.ToErrors();
        }

        var position = positionResult.Value;

        if (!position.IsActive)
        {
            return UnitResult.Success<Errors>();
        }

        position.Deactivate();

        var saveResult =
            await transactionManager.SaveChangesAsync(
                cancellationToken);

        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        logger.LogInformation(
            "Позиция {PositionId} удалена",
            position.Id.Value);

        return UnitResult.Success<Errors>();
    }
}