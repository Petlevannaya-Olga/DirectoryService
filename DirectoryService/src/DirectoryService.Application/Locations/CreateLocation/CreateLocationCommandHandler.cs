using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.CreateLocation;

public sealed class CreateLocationCommandHandler(
    ILocationsRepository repository,
    ILogger<CreateLocationCommandHandler> logger)
    : ICommandHandler<Guid, CreateLocationCommand>
{
    public async Task<Result<Guid, Errors>> Handle(
        CreateLocationCommand command,
        CancellationToken cancellationToken)
    {
        var locationNameResult = LocationName.Create(command.Dto.Name);

        if (locationNameResult.IsFailure)
        {
            return locationNameResult.Error.ToErrors();
        }

        var addressResult = Address.Create(command.Dto.Address);

        if (addressResult.IsFailure)
        {
            return addressResult.Error.ToErrors();
        }

        var timezoneResult = Timezone.Create(command.Dto.Timezone);

        if (timezoneResult.IsFailure)
        {
            return timezoneResult.Error.ToErrors();
        }

        var address = addressResult.Value;
        var locationName = locationNameResult.Value;
        var timezone = timezoneResult.Value;
        
        var addressExistsResult = await repository.ExistsAsync(
            location =>
                location.Address.PostalCode == address.PostalCode &&
                location.Address.City == address.City &&
                location.Address.Region == address.Region &&
                location.Address.Street == address.Street &&
                location.Address.House == address.House &&
                location.Address.Apartment == address.Apartment,
            cancellationToken);

        if (addressExistsResult.IsFailure)
        {
            return addressExistsResult.Error.ToErrors();
        }

        if (addressExistsResult.Value)
        {
            logger.LogWarning(
                "Локация с адресом {@Address} уже существует",
                address);

            return LocationErrors
                .AddressConflict()
                .ToErrors();
        }

        var nameExistsResult = await repository.ExistsAsync(
            location => location.Name == locationName,
            cancellationToken);

        if (nameExistsResult.IsFailure)
        {
            return nameExistsResult.Error.ToErrors();
        }

        if (nameExistsResult.Value)
        {
            logger.LogWarning(
                "Локация с именем {LocationName} уже существует",
                locationName);

            return LocationErrors
                .NameConflict()
                .ToErrors();
        }

        var location = new Location(
            locationName,
            address,
            timezone);

        var addResult = await repository.AddAsync(
            location,
            cancellationToken);

        if (addResult.IsFailure)
        {
            var errors = addResult.Error.ToErrors();

            logger.LogError(
                "Ошибка сохранения локации в БД: {@Errors}",
                errors);

            return errors;
        }

        logger.LogInformation(
            "Создана локация с id = {LocationId}",
            location.Id.Value);

        return location.Id.Value;
    }
}