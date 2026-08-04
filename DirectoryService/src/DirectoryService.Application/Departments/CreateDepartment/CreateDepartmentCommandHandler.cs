using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.CreateDepartment;

public sealed class CreateDepartmentCommandHandler(
    ILocationsRepository locationsRepository,
    IDepartmentsRepository departmentsRepository,
    ILogger<CreateDepartmentCommandHandler> logger)
    : ICommandHandler<Guid, CreateDepartmentCommand>
{
    public async Task<Result<Guid, Errors>> Handle(
        CreateDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var nameResult = DepartmentName.Create(command.Dto.Name);

        if (nameResult.IsFailure)
        {
            return nameResult.Error.ToErrors();
        }

        var slugResult = Slug.Create(command.Dto.Name);

        if (slugResult.IsFailure)
        {
            return slugResult.Error.ToErrors();
        }

        var locationIds = command.Dto.LocationIds
            .Distinct()
            .ToList();

        var locationsExistResult = await locationsRepository.ExistsAsync(
            locationIds,
            cancellationToken);

        if (locationsExistResult.IsFailure)
        {
            return locationsExistResult.Error.ToErrors();
        }

        var locations = locationIds
            .Select(id => new LocationId(id))
            .ToList();

        var createResult = await CreateDepartmentAsync(
            command.Dto.ParentId,
            nameResult.Value,
            slugResult.Value,
            locations,
            cancellationToken);

        if (createResult.IsFailure)
        {
            return createResult.Error;
        }

        var department = createResult.Value;

        var addResult = await departmentsRepository.AddAsync(
            department,
            cancellationToken);

        if (addResult.IsFailure)
        {
            return addResult.Error.ToErrors();
        }

        logger.LogInformation(
            "Создано подразделение с id = {DepartmentId}",
            department.Id.Value);

        return department.Id.Value;
    }

    private async Task<Result<Department, Errors>> CreateDepartmentAsync(
        Guid? parentId,
        DepartmentName name,
        Slug slug,
        IReadOnlyCollection<LocationId> locationIds,
        CancellationToken cancellationToken)
    {
        Result<Department, Error> createResult;

        if (parentId is null)
        {
            createResult = Department.CreateParent(
                name,
                slug,
                locationIds);
        }
        else
        {
            var parentResult = await departmentsRepository.GetByIdAsync(
                parentId.Value,
                cancellationToken);

            if (parentResult.IsFailure)
            {
                return parentResult.Error.ToErrors();
            }

            createResult = Department.CreateChild(
                name,
                slug,
                parentResult.Value,
                locationIds);
        }

        if (createResult.IsFailure)
        {
            return createResult.Error.ToErrors();
        }

        return createResult.Value;
    }
}