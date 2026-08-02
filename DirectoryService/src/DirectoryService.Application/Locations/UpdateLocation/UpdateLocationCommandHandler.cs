using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Locations;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.UpdateLocation;

public sealed class UpdateLocationHandler(
    ILocationsRepository locationsRepository,
    ITransactionManager transactionManager)
    : ICommandHandler<Guid, UpdateLocationCommand>
{
    public async Task<Result<Guid, Errors>> Handle(
        UpdateLocationCommand command,
        CancellationToken cancellationToken)
    {
        var locationId = new LocationId(command.Id);

        var locationResult = await locationsRepository.GetByAsync(
            x => x.Id == locationId,
            cancellationToken);

        if (locationResult.IsFailure)
        {
            return locationResult.Error.ToErrors();
        }

        var location = locationResult.Value;

        if (location is null)
        {
            return CommonErrors
                .Failure("location.was.not.found", "Локация не найдена")
                .ToErrors();
        }

        var nameResult = LocationName.Create(command.Name);
        var addressResult = Address.Create(command.Address);
        var timezoneResult = Timezone.Create(command.Timezone);

        location.Update(
            nameResult.Value,
            addressResult.Value,
            timezoneResult.Value);

        var saveResult = await transactionManager.SaveChangesAsync(cancellationToken);

        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        return command.Id;
    }
}