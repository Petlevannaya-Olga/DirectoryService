using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.UpdateLocations;

public sealed class UpdateLocationsCommandHandler(
    IDepartmentsRepository departmentsRepository,
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager,
    ILogger<UpdateLocationsCommandHandler> logger)
    : ICommandHandler<Guid, UpdateLocationsCommand>
{
    public async Task<Result<Guid, Errors>> Handle(
        UpdateLocationsCommand command,
        CancellationToken cancellationToken)
    {
        var departmentResult =
            await departmentsRepository.GetByIdWithLocationsAsync(
                command.DepartmentId,
                cancellationToken);

        if (departmentResult.IsFailure)
        {
            return departmentResult.Error.ToErrors();
        }

        var department = departmentResult.Value;

        if (!department.IsActive)
        {
            logger.LogWarning(
                "Подразделение с id = {DepartmentId} неактивно",
                department.Id.Value);

            return CommonErrors
                .Inactive(department.Id.Value)
                .ToErrors();
        }

        var locationsResult =
            await locationsRepository.ExistsAndActiveAsync(
                command.LocationIds,
                cancellationToken);

        if (locationsResult.IsFailure)
        {
            return locationsResult.Error.ToErrors();
        }

        department.UpdateLocations(command.LocationIds);

        var saveResult =
            await transactionManager.SaveChangesAsync(
                cancellationToken);

        if (saveResult.IsFailure)
        {
            logger.LogError(
                "Не удалось обновить локации подразделения с id = {DepartmentId}",
                department.Id.Value);

            return saveResult.Error.ToErrors();
        }

        return department.Id.Value;
    }
}