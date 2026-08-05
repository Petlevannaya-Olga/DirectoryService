using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Locations;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.CreateLocation;

public sealed class CreateLocationHandler(
    ILocationsRepository repository,
    ITransactionManager transactionManager,
    ILogger<CreateLocationHandler> logger)
    : ICommandHandler<Guid, CreateLocationCommand>
{
    public async Task<Result<Guid, Errors>> Handle(
        CreateLocationCommand command,
        CancellationToken cancellationToken)
    {
        var locationNameResult =
            LocationName.Create(command.Dto.Name);

        if (locationNameResult.IsFailure)
        {
            return locationNameResult.Error.ToErrors();
        }

        var addressResult =
            Address.Create(command.Dto.Address);

        if (addressResult.IsFailure)
        {
            return addressResult.Error.ToErrors();
        }

        var timezoneResult =
            Timezone.Create(command.Dto.Timezone);

        if (timezoneResult.IsFailure)
        {
            return timezoneResult.Error.ToErrors();
        }

        var locationName = locationNameResult.Value;
        var address = addressResult.Value;
        var timezone = timezoneResult.Value;

        var addressExistsResult =
            await repository.ExistsAsync(
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

        var nameExistsResult =
            await repository.ExistsAsync(
                location => location.Name == locationName,
                cancellationToken);

        if (nameExistsResult.IsFailure)
        {
            return nameExistsResult.Error.ToErrors();
        }

        if (nameExistsResult.Value)
        {
            logger.LogWarning(
                "Локация с названием {LocationName} уже существует",
                locationName.Value);

            return LocationErrors
                .NameConflict()
                .ToErrors();
        }

        var location = new Location(
            locationName,
            address,
            timezone);

        var addResult =
            await repository.AddAsync(
                location,
                cancellationToken);

        if (addResult.IsFailure)
        {
            return addResult.Error.ToErrors();
        }

        var saveResult =
            await transactionManager.SaveChangesAsync(
                cancellationToken);

        if (saveResult.IsFailure)
        {
            logger.LogError(
                "Ошибка сохранения локации. Код ошибки: {ErrorCode}",
                saveResult.Error.Code);

            return saveResult.Error.ToErrors();
        }

        logger.LogInformation(
            "Создана локация с id = {LocationId}",
            location.Id.Value);

        return location.Id.Value;
    }
}