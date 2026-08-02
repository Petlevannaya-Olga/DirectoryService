using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Locations;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.AddLocation;

public sealed class AddLocationCommandHandler(
    IDepartmentsRepository departmentsRepository,
    ILocationsRepository locationsRepository,
    ITransactionScope unitOfWork)
    : ICommandHandler<Guid, AddLocationCommand>
{
    public async Task<Result<Guid, Errors>> Handle(
        AddLocationCommand command,
        CancellationToken cancellationToken)
    {
        var locationId = new LocationId(command.LocationId);

        var departmentResult =
            await departmentsRepository.GetByIdWithLocationsAsync(
                command.DepartmentId,
                cancellationToken);

        if (departmentResult.IsFailure)
        {
            return departmentResult.Error.ToErrors();
        }

        if (departmentResult.Value == null)
        {
            return CommonErrors
                .NotFound(
                "department.not.found",
                $"Подразделение с идентификатором '{command.DepartmentId}' не найдено")
                .ToErrors();
        }

        var locationExistsResult =
            await locationsRepository.EnsureExistsAndActiveAsync(
                locationId,
                cancellationToken);

        if (locationExistsResult.IsFailure)
        {
            return locationExistsResult.Error.ToErrors();
        }

        var department = departmentResult.Value;

        var addLocationResult = department.AddLocation(locationId);

        if (addLocationResult.IsFailure)
        {
            return addLocationResult.Error.ToErrors();
        }

        unitOfWork.Commit();

        return department.Id.Value;
    }
}