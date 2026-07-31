using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.CreateLocation;

public class CreateLocationCommandHandler(
    ILocationsRepository repository,
    ILogger<CreateLocationCommandHandler> logger)
    : ICommandHandler<Guid, CreateLocationCommand>
{
    public async Task<Result<Guid, Errors>> Handle(CreateLocationCommand command, CancellationToken cancellationToken)
    {
        var locationName = LocationName.Create(command.Dto.Name);
        var address = Address.Create(command.Dto.Address);
        var timezone = Timezone.Create(command.Dto.Timezone);

        var getResult = await repository.GetByAsync(
            l =>
                l.Address.PostalCode == address.Value.PostalCode &&
                l.Address.City == address.Value.City &&
                l.Address.Region == address.Value.Region &&
                l.Address.Street == address.Value.Street &&
                l.Address.House == address.Value.House &&
                l.Address.Apartment == address.Value.Apartment,
            cancellationToken);

        if (getResult.IsFailure)
        {
            return getResult.Error.ToErrors();
        }

        if (getResult.Value is not null)
        {
            logger.LogError("Локация с таким адресом уже существует");
            return LocationErrors.LocationAddressConflict().ToErrors();
        }

        var location = new Location(
            locationName.Value,
            address.Value,
            timezone.Value,
            []);

        var addResult = await repository.AddAsync(location, cancellationToken);

        if (addResult.IsFailure)
        {
            var errors = addResult.Error.ToErrors();
            logger.LogError("Ошибка сохранения локации в БД: {@Errors}", errors);
            return errors;
        }

        logger.LogInformation("Создана локация с id = {locationId}", location.Id);

        return location.Id.Value;
    }

    [ExcludeFromCodeCoverage]
    private static class LocationErrors
    {
        public static Error LocationAddressConflict() =>
            CommonErrors.Conflict(
                "location.address.conflict",
                $"Локация с таким адресом уже существует");
    }
}