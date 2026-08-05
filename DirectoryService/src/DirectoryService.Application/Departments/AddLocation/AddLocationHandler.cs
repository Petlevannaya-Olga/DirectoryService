using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Locations;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.AddLocation;

public sealed class AddLocationHandler(
    IDepartmentsRepository departmentsRepository,
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager,
    ILogger<AddLocationHandler> logger)
    : ICommandHandler<Guid, AddLocationCommand>
{
    public async Task<Result<Guid, Errors>> Handle(
        AddLocationCommand command,
        CancellationToken cancellationToken)
    {
        var locationId = new LocationId(command.LocationId);

        var transactionResult =
            await transactionManager.BeginTransactionAsync(
                cancellationToken);

        if (transactionResult.IsFailure)
        {
            return transactionResult.Error.ToErrors();
        }

        await using var transaction =
            transactionResult.Value;

        var locationResult =
            await locationsRepository.EnsureExistsAndActiveForUpdateAsync(
                locationId,
                cancellationToken);

        if (locationResult.IsFailure)
        {
            return locationResult.Error.ToErrors();
        }

        var departmentResult =
            await departmentsRepository.GetByIdWithLocationsAsync(
                command.DepartmentId,
                cancellationToken);

        if (departmentResult.IsFailure)
        {
            return departmentResult.Error.ToErrors();
        }

        var department = departmentResult.Value;

        var addLocationResult =
            department.AddLocation(locationId);

        if (addLocationResult.IsFailure)
        {
            return addLocationResult.Error.ToErrors();
        }

        var saveResult =
            await transactionManager.SaveChangesAsync(
                cancellationToken);

        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        var commitResult =
            await transaction.CommitAsync(cancellationToken);

        if (commitResult.IsFailure)
        {
            return commitResult.Error.ToErrors();
        }

        var departmentId = department.Id.Value;

        logger.LogInformation(
            "Локация {LocationId} успешно добавлена к подразделению {DepartmentId}",
            locationId.Value,
            departmentId);

        return departmentId;
    }
}