using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.CreateDepartmentCommand;

public class CreateDepartmentCommandHandler(
    ILocationsRepository locationsRepository,
    IDepartmentsRepository departmentsRepository,
    ILogger<CreateDepartmentCommandHandler> logger) : ICommandHandler<Guid, CreateDepartmentCommand>
{
    public async Task<Result<Guid, Errors>>
        Handle(CreateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var departmentId = new DepartmentId(Guid.NewGuid());
        var nameResult = DepartmentName.Create(command.Dto.Name);
        var identifierResult = Identifier.Create(command.Dto.Name);

        var locationIds = command.Dto.LocationIds.ToList();

        var existsResult = await locationsRepository.ExistsAsync(locationIds, cancellationToken);

        if (existsResult.IsFailure)
            return existsResult.Error.ToErrors();

        var locations = locationIds
            .Select(x => new DepartmentLocation(
                new DepartmentId(Guid.NewGuid()),
                new LocationId(x)))
            .ToList();

        var parentId = command.Dto.ParentId;

        var createResult = await CreateDepartment(
            parentId: parentId,
            name: nameResult.Value,
            identifier: identifierResult.Value,
            locations: locations,
            cancellationToken: cancellationToken);

        if (createResult.IsFailure)
        {
            return createResult.Error;
        }

        var addResult = await departmentsRepository.AddAsync(createResult.Value, cancellationToken);

        if (addResult.IsFailure)
            return addResult.Error.ToErrors();

        logger.LogInformation("Создано подразделение с id = {DepartmentId}", departmentId);

        return departmentId.Value;
    }

    private async Task<Result<Department, Errors>> CreateDepartment(
        Guid? parentId,
        DepartmentName name,
        Identifier identifier,
        IEnumerable<DepartmentLocation> locations,
        CancellationToken cancellationToken)
    {
        if (parentId is null)
        {
            var createParentResult = Department.CreateParent(
                name,
                identifier,
                locations);

            if (createParentResult.IsFailure)
            {
                return createParentResult.Value;
            }

            return createParentResult.Error.ToErrors();
        }

        var departmentParent = await departmentsRepository
            .GetByIdAsync(parentId.Value, cancellationToken);

        if (departmentParent.IsFailure)
        {
            return departmentParent.Error.ToErrors();
        }

        if (departmentParent.Value is null)
        {
            logger.LogError("Подразделение с id = {Id} не найдено", parentId.Value);
            return DepartmentErrors.DepartmentNotFound(parentId.Value).ToErrors();
        }

        var createChildResult = Department.CreateChild(
            name,
            identifier,
            departmentParent.Value,
            locations);

        if (createChildResult.IsFailure)
        {
            return createChildResult.Error.ToErrors();
        }

        return createChildResult.Value;
    }

    [ExcludeFromCodeCoverage]
    private static class DepartmentErrors
    {
        public static Error DepartmentNotFound(Guid id)
        {
            return CommonErrors.NotFound(
                "department.not.found",
                $"Подразделение с id = {id} не найдено",
                id);
        }
    }
}