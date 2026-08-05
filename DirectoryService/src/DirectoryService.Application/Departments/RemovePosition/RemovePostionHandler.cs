using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Departments;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.RemovePosition;

public sealed class RemovePositionHandler(
    IPositionsRepository positionsRepository,
    ITransactionManager transactionManager,
    ILogger<RemovePositionHandler> logger)
    : ICommandHandler<RemovePositionCommand>
{
    public async Task<UnitResult<Errors>> Handle(
        RemovePositionCommand command,
        CancellationToken cancellationToken)
    {
        var departmentId =
            new DepartmentId(command.DepartmentId);

        var positionResult =
            await positionsRepository.GetByIdWithDepartmentsAsync(
                command.PositionId,
                cancellationToken);

        if (positionResult.IsFailure)
        {
            return positionResult.Error.ToErrors();
        }

        var position = positionResult.Value;

        var removeResult =
            position.RemoveDepartment(departmentId);

        if (removeResult.IsFailure)
        {
            return removeResult.Error.ToErrors();
        }

        // Связи уже нет — состояние уже соответствует DELETE-запросу.
        if (!removeResult.Value)
        {
            return UnitResult.Success<Errors>();
        }

        var saveResult =
            await transactionManager.SaveChangesAsync(
                cancellationToken);

        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        logger.LogInformation(
            "Позиция {PositionId} отвязана от подразделения {DepartmentId}",
            position.Id.Value,
            departmentId.Value);

        return UnitResult.Success<Errors>();
    }
}