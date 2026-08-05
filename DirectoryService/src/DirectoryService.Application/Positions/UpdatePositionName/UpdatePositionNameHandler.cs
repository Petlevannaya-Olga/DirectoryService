using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Positions;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Positions.UpdatePositionName;

public sealed class UpdatePositionNameHandler(
    IPositionsRepository positionsRepository,
    ITransactionManager transactionManager,
    ILogger<UpdatePositionNameHandler> logger)
    : ICommandHandler<Guid, UpdatePositionNameCommand>
{
    public async Task<Result<Guid, Errors>> Handle(
        UpdatePositionNameCommand command,
        CancellationToken cancellationToken)
    {
        var nameResult =
            PositionName.Create(command.Dto.Name);

        if (nameResult.IsFailure)
        {
            return nameResult.Error.ToErrors();
        }

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
        var newName = nameResult.Value;

        if (position.Name == newName)
        {
            return position.Id.Value;
        }

        if (position.IsActive)
        {
            var nameExistsResult =
                await positionsRepository.ExistsAsync(
                    otherPosition =>
                        otherPosition.Id != positionId &&
                        otherPosition.Name == newName &&
                        otherPosition.IsActive,
                    cancellationToken);

            if (nameExistsResult.IsFailure)
            {
                return nameExistsResult.Error.ToErrors();
            }

            if (nameExistsResult.Value)
            {
                logger.LogWarning(
                    "Активная позиция с названием {PositionName} уже существует",
                    newName.Value);

                return PositionErrors
                    .NameConflict(newName.Value)
                    .ToErrors();
            }
        }

        position.UpdateName(newName);

        var saveResult =
            await transactionManager.SaveChangesAsync(
                cancellationToken);

        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        logger.LogInformation(
            "Позиция {PositionId} переименована в {PositionName}",
            position.Id.Value,
            newName.Value);

        return position.Id.Value;
    }
}