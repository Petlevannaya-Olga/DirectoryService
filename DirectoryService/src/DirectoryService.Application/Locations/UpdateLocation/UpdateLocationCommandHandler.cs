using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Locations;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.UpdateLocation;

public sealed class UpdateLocationCommandHandler(
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
            location => location.Id == locationId,
            cancellationToken);

        if (locationResult.IsFailure)
        {
            return locationResult.Error.ToErrors();
        }

        var location = locationResult.Value;

        var name = LocationName.Create(command.Name).Value;
        var address = Address.Create(command.Address).Value;
        var timezone = Timezone.Create(command.Timezone).Value;

        var nameExistsResult = await locationsRepository.ExistsAsync(
            otherLocation =>
                otherLocation.Id != locationId &&
                otherLocation.Name == name,
            cancellationToken);

        if (nameExistsResult.IsFailure)
        {
            return nameExistsResult.Error.ToErrors();
        }

        if (nameExistsResult.Value)
        {
            return LocationErrors
                .NameConflict()
                .ToErrors();
        }

        var addressExistsResult = await locationsRepository.ExistsAsync(
            otherLocation =>
                otherLocation.Id != locationId &&
                otherLocation.Address.PostalCode == address.PostalCode &&
                otherLocation.Address.City == address.City &&
                otherLocation.Address.Region == address.Region &&
                otherLocation.Address.Street == address.Street &&
                otherLocation.Address.House == address.House &&
                otherLocation.Address.Apartment == address.Apartment,
            cancellationToken);

        if (addressExistsResult.IsFailure)
        {
            return addressExistsResult.Error.ToErrors();
        }

        if (addressExistsResult.Value)
        {
            return LocationErrors
                .AddressConflict()
                .ToErrors();
        }

        location.Update(
            name,
            address,
            timezone);

        var saveResult = await transactionManager.SaveChangesAsync(
            cancellationToken);

        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        return location.Id.Value;
    }
}