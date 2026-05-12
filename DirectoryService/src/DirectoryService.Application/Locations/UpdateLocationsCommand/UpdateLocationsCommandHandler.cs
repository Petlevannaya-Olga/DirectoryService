using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.UpdateLocationsCommand;

public class UpdateLocationsCommandHandler(
    IDepartmentsRepository departmentsRepository,
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager,
    ILogger<UpdateLocationsCommandHandler> logger) : ICommandHandler<Guid, UpdateLocationsCommand>
{
    public async Task<Result<Guid, Errors>> Handle(UpdateLocationsCommand command, CancellationToken cancellationToken)
    {
        var getDepartmentResult = await departmentsRepository.GetByIdWithLocationsAsync(command.DepartmentId, cancellationToken);

        if (getDepartmentResult.IsFailure)
        {
            return getDepartmentResult.Error.ToErrors();
        }

        var department = getDepartmentResult.Value;

        if (department is null)
        {
            logger.LogError("Департамент с id = {Id} не найден", command.DepartmentId);
            return CommonErrors
                .NotFound(
                    "department.was.not.found",
                    "Департамент не найден",
                    command.DepartmentId).ToErrors();
        }

        if (!department.IsActive)
        {
            logger.LogError("Департамент с id = {Id} неактивен", command.DepartmentId);
            return CommonErrors.Inactive(command.DepartmentId).ToErrors();
        }

        var locationsExistsResult = await locationsRepository.ExistsAndActiveAsync(command.LocationIds, cancellationToken);

        if (locationsExistsResult.IsFailure)
        {
            return locationsExistsResult.Error.ToErrors();
        }

        department.UpdateLocations(command.LocationIds);

        var saveChangesResult = await transactionManager.SaveChangesAsync(cancellationToken);

        if (saveChangesResult.IsSuccess)
        {
            return command.DepartmentId;
        }

        logger.LogError("Не удалось обновить локации для департамента с id = {Id}", command.DepartmentId);
        return saveChangesResult.Error.ToErrors();
    }
}