using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.AddPosition;

public sealed class AddPositionHandler(
    IDepartmentsRepository departmentsRepository,
    IPositionsRepository positionsRepository,
    ITransactionManager transactionManager,
    ILogger<AddPositionHandler> logger)
    : ICommandHandler<Guid, AddPositionCommand>
{
    public async Task<Result<Guid, Errors>> Handle(
        AddPositionCommand command,
        CancellationToken cancellationToken)
    {
        var departmentId =
            new DepartmentId(command.DepartmentId);

        var positionId =
            new PositionId(command.PositionId);

        var departmentsResult =
            await departmentsRepository.ExistsAndActive(
                [command.DepartmentId],
                cancellationToken);

        if (departmentsResult.IsFailure)
        {
            return departmentsResult.Error.ToErrors();
        }

        var positionResult =
            await positionsRepository.GetByIdWithDepartmentsAsync(
                command.PositionId,
                cancellationToken);

        if (positionResult.IsFailure)
        {
            return positionResult.Error.ToErrors();
        }

        var position = positionResult.Value;

        if (!position.IsActive)
        {
            logger.LogWarning(
                "Позиция {PositionId} неактивна",
                positionId.Value);

            return CommonErrors
                .Inactive(positionId.Value)
                .ToErrors();
        }

        var addDepartmentResult =
            position.AddDepartment(departmentId);

        if (addDepartmentResult.IsFailure)
        {
            return addDepartmentResult.Error.ToErrors();
        }

        var saveResult =
            await transactionManager.SaveChangesAsync(
                cancellationToken);

        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        logger.LogInformation(
            "Позиция {PositionId} привязана к подразделению {DepartmentId}",
            positionId.Value,
            departmentId.Value);

        return departmentId.Value;
    }
}