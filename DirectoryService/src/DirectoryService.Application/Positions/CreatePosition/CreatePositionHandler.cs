using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Positions.CreatePosition;

public sealed class CreatePositionHandler(
    IDepartmentsRepository departmentsRepository,
    IPositionsRepository positionsRepository,
    ITransactionManager transactionManager,
    ILogger<CreatePositionHandler> logger)
    : ICommandHandler<Guid, CreatePositionCommand>
{
    public async Task<Result<Guid, Errors>> Handle(
        CreatePositionCommand command,
        CancellationToken cancellationToken)
    {
        var nameResult =
            PositionName.Create(command.Dto.Name);

        if (nameResult.IsFailure)
        {
            return nameResult.Error.ToErrors();
        }

        var descriptionResult =
            Description.Create(command.Dto.Description);

        if (descriptionResult.IsFailure)
        {
            return descriptionResult.Error.ToErrors();
        }

        var departmentIds = command.Dto.DepartmentIds
            .Distinct()
            .ToArray();

        if (departmentIds.Length == 0)
        {
            return PositionErrors
                .DepartmentsRequired()
                .ToErrors();
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
                "Активная позиция с названием {PositionName} уже существует",
                name.Value);

            return PositionErrors
                .NameConflict(name.Value)
                .ToErrors();
        }

        var departmentsResult =
            await departmentsRepository.ExistsAndActive(
                departmentIds,
                cancellationToken);

        if (departmentsResult.IsFailure)
        {
            return departmentsResult.Error.ToErrors();
        }

        var departments = departmentIds
            .Select(id => new DepartmentId(id))
            .ToArray();

        var createResult = Position.Create(
            name,
            description,
            departments);

        if (createResult.IsFailure)
        {
            return createResult.Error.ToErrors();
        }

        var position = createResult.Value;

        var addResult =
            await positionsRepository.AddAsync(
                position,
                cancellationToken);

        if (addResult.IsFailure)
        {
            return addResult.Error.ToErrors();
        }

        var saveResult =
            await transactionManager.SaveChangesAsync(
                cancellationToken);

        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        logger.LogInformation(
            "Создана позиция с id = {PositionId}",
            position.Id.Value);

        return position.Id.Value;
    }
}