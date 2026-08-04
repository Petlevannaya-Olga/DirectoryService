using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Positions.CreatePosition;

public sealed class CreatePositionCommandHandler(
    IDepartmentsRepository departmentsRepository,
    IPositionsRepository positionsRepository,
    ILogger<CreatePositionCommandHandler> logger)
    : ICommandHandler<Guid, CreatePositionCommand>
{
    public async Task<Result<Guid, Errors>> Handle(
        CreatePositionCommand command,
        CancellationToken cancellationToken)
    {
        var nameResult = PositionName.Create(command.Dto.Name);

        if (nameResult.IsFailure)
        {
            return nameResult.Error.ToErrors();
        }

        var descriptionResult = Description.Create(command.Dto.Description);

        if (descriptionResult.IsFailure)
        {
            return descriptionResult.Error.ToErrors();
        }

        var name = nameResult.Value;
        var description = descriptionResult.Value;
        
        var positionExistsResult =
            await positionsRepository.ExistsAsync(
                position =>
                    position.Name == name &&
                    position.IsActive,
                cancellationToken);

        if (positionExistsResult.IsFailure)
        {
            return positionExistsResult.Error.ToErrors();
        }

        if (positionExistsResult.Value)
        {
            logger.LogWarning(
                "Активная позиция с названием '{PositionName}' уже существует",
                command.Dto.Name);

            return PositionErrors
                .NameConflict(command.Dto.Name)
                .ToErrors();
        }

        var departmentIds = command.Dto.DepartmentIds
            .Distinct()
            .ToList();

        var departmentsResult =
            await departmentsRepository.ExistsAndActive(
                departmentIds,
                cancellationToken);

        if (departmentsResult.IsFailure)
        {
            return departmentsResult.Error.ToErrors();
        }

        var departments = departmentIds
            .ConvertAll(id => new DepartmentId(id))
;

        var createResult = Position.Create(
            name,
            description,
            departments);

        if (createResult.IsFailure)
        {
            return createResult.Error.ToErrors();
        }

        var position = createResult.Value;

        var addResult = await positionsRepository.AddAsync(
            position,
            cancellationToken);

        if (addResult.IsFailure)
        {
            return addResult.Error.ToErrors();
        }

        logger.LogInformation(
            "Создана позиция с id = {PositionId}",
            position.Id.Value);

        return position.Id.Value;
    }
}