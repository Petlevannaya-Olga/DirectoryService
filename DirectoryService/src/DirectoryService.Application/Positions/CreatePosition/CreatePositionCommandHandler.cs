using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Positions.CreatePosition;

public class CreatePositionCommandHandler(
    IDepartmentsRepository departmentsRepository,
    IPositionsRepository positionsRepository,
    ILogger<CreatePositionCommandHandler> logger) : ICommandHandler<Guid, CreatePositionCommand>
{
    public async Task<Result<Guid, Errors>> Handle(CreatePositionCommand command, CancellationToken cancellationToken)
    {
        var positionId = new PositionId(Guid.NewGuid());

        var nameResult = PositionName.Create(command.Dto.Name);

        var getResult = await positionsRepository
            .GetByAsync(x => x.Name == nameResult.Value, cancellationToken);

        if (getResult.IsFailure)
        {
            return getResult.Error.ToErrors();
        }

        if (getResult.Value is not null && getResult.Value.IsActive)
        {
            var errors = PositionErrors.PositionNameConflict(command.Dto.Name).ToErrors();
            logger.LogError(
                "Нельзя добавить позицию с названием '{PositionName}', т.к. она уже существует и активна",
                command.Dto.Name);
            return errors;
        }

        var descriptionResult = Description.Create(command.Dto.Description);

        if (descriptionResult.IsFailure)
        {
            return descriptionResult.Error.ToErrors();
        }

        var existAndActiveResult = await departmentsRepository
            .ExistsAndActive(command.Dto.DepartmentIds, cancellationToken);

        if (existAndActiveResult.IsFailure)
        {
            return existAndActiveResult.Error.ToErrors();
        }

        var departmentPositions = command.Dto.DepartmentIds
            .Select(x => new DepartmentPosition(new DepartmentId(x), positionId));

        var newPosition = new Position(
            nameResult.Value,
            descriptionResult.Value,
            departmentPositions);

        var addResult = await positionsRepository.AddAsync(newPosition, cancellationToken);

        if (addResult.IsFailure)
        {
            return addResult.Error.ToErrors();
        }

        logger.LogInformation("Создана позиция с id = {PositionId}", positionId);

        return positionId.Value;
    }

    [ExcludeFromCodeCoverage]
    private static class PositionErrors
    {
        public static Error PositionNameConflict(string positionName)
        {
            return CommonErrors.Conflict(
                "position.name.conflict",
                $"Позиция с названием {positionName} уже существует и активна");
        }
    }
}