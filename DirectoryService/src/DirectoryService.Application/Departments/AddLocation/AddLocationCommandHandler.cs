using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Locations;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.AddLocation;

public sealed class AddLocationCommandHandler(
    IDepartmentsRepository departmentsRepository,
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager)
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

        using var transactionScope = transactionResult.Value;

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

        var addLocationResult = department.AddLocation(locationId);

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

        transactionScope.Commit();

        return department.Id.Value;
    }
}