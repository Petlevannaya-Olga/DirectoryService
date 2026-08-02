using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Locations;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.RemoveLocation;

public sealed class RemoveLocationCommandHandler(
    IDepartmentsRepository departmentsRepository,
    ITransactionScope transactionScope)
    : ICommandHandler<RemoveLocationCommand>
{
    public async Task<UnitResult<Errors>> Handle(RemoveLocationCommand command, CancellationToken cancellationToken)
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

        var removeResult =
            departmentResult.Value.RemoveLocation(locationId);

        if (removeResult.IsFailure)
        {
            return removeResult.Error.ToErrors();
        }

        transactionScope.Commit();

        return UnitResult.Success<Errors>();
    }
}