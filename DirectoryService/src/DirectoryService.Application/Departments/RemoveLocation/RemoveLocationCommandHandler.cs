using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Locations;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.RemoveLocation;

public sealed class RemoveLocationCommandHandler(
    IDepartmentsRepository departmentsRepository,
    ITransactionManager transactionManager,
    ILogger<RemoveLocationCommandHandler> logger)
    : ICommandHandler<RemoveLocationCommand>
{
    public async Task<UnitResult<Errors>> Handle(
        RemoveLocationCommand command,
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

        var department = departmentResult.Value;

        var removeResult =
            department.RemoveLocation(locationId);

        if (removeResult.IsFailure)
        {
            return removeResult.Error.ToErrors();
        }

        var saveResult =
            await transactionManager.SaveChangesAsync(
                cancellationToken);

        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        logger.LogInformation(
            "Локация {LocationId} удалена из подразделения {DepartmentId}",
            command.LocationId,
            command.DepartmentId);

        return UnitResult.Success<Errors>();
    }
}