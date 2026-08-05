using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Locations;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.DeleteLocation;

public sealed class DeleteLocationHandler(
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager,
    ILogger<DeleteLocationHandler> logger)
    : ICommandHandler<DeleteLocationCommand>
{
    public async Task<UnitResult<Errors>> Handle(
        DeleteLocationCommand command,
        CancellationToken cancellationToken)
    {
        var locationId = new LocationId(command.LocationId);

        var locationResult =
            await locationsRepository.GetByAsync(
                location => location.Id == locationId,
                cancellationToken);

        if (locationResult.IsFailure)
        {
            return locationResult.Error.ToErrors();
        }

        var location = locationResult.Value;

        if (!location.IsActive)
        {
            return UnitResult.Success<Errors>();
        }

        location.Deactivate();

        var saveResult =
            await transactionManager.SaveChangesAsync(
                cancellationToken);

        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        logger.LogInformation(
            "Локация {LocationId} удалена",
            location.Id.Value);

        return UnitResult.Success<Errors>();
    }
}