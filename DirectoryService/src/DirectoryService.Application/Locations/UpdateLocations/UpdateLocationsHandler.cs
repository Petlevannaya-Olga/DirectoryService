using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.UpdateLocations;

public sealed class UpdateLocationsHandler(
    IDepartmentsRepository departmentsRepository,
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager,
    ILogger<UpdateLocationsHandler> logger)
    : ICommandHandler<Guid, UpdateLocationsCommand>
{
    public async Task<Result<Guid, Errors>> Handle(
        UpdateLocationsCommand command,
        CancellationToken cancellationToken)
    {
        var locationIds = command.LocationIds
            .Distinct()
            .ToArray();

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
                "Подразделение {DepartmentId} неактивно",
                department.Id.Value);

            return CommonErrors
                .Inactive(department.Id.Value)
                .ToErrors();
        }

        var locationsResult =
            await locationsRepository.ExistsAndActiveAsync(
                locationIds,
                cancellationToken);

        if (locationsResult.IsFailure)
        {
            return locationsResult.Error.ToErrors();
        }

        department.UpdateLocations(locationIds);

        var saveResult =
            await transactionManager.SaveChangesAsync(
                cancellationToken);

        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        logger.LogInformation(
            "Локации подразделения {DepartmentId} успешно обновлены",
            department.Id.Value);

        return department.Id.Value;
    }
}