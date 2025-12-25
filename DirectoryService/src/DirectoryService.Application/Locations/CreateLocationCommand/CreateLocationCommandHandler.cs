using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.CreateLocationCommand;

public class CreateLocationCommandHandler(
    ILogger<CreateLocationCommandHandler> logger,
    ILocationsRepository repository)
    : ICommandHandler<Guid, CreateLocationCommand>
{
    public async Task<Result<Guid, Failure>> Handle(CreateLocationCommand command, CancellationToken cancellationToken)
    {
        var locationNameCreateResult = LocationName.Create(command.Dto.Name);
        if (locationNameCreateResult.IsFailure)
        {
            return locationNameCreateResult.Error.ToFailure();
        }

        var addressCreateResult = Address.Create(
            command.Dto.Address.PostalCode,
            command.Dto.Address.Region,
            command.Dto.Address.City,
            command.Dto.Address.Street,
            command.Dto.Address.House,
            command.Dto.Address.Apartment);

        if (addressCreateResult.IsFailure)
        {
            return addressCreateResult.Error.ToFailure();
        }

        var timezoneCreateResult = Timezone.Create(command.Dto.Timezone);

        if (timezoneCreateResult.IsFailure)
        {
            return timezoneCreateResult.Error.ToFailure();
        }

        var location = new Location(
            locationNameCreateResult.Value,
            addressCreateResult.Value,
            timezoneCreateResult.Value,
            []);

        await repository.AddAsync(location, cancellationToken);

        logger.LogInformation("Создана локация с id = {locationId}", location.Id);

        return location.Id;
    }
}