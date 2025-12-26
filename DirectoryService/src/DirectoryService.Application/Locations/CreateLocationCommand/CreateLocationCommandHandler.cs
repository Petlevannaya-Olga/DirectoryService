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
    public async Task<Result<Guid, Errors>> Handle(CreateLocationCommand command, CancellationToken cancellationToken)
    {
        var locationNameCreateResult = LocationName.Create(command.Dto.Name);
        if (locationNameCreateResult.IsFailure)
        {
            return locationNameCreateResult.Error.ToErrors();
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
            return addressCreateResult.Error.ToErrors();
        }

        var timezoneCreateResult = Timezone.Create(command.Dto.Timezone);

        if (timezoneCreateResult.IsFailure)
        {
            return timezoneCreateResult.Error.ToErrors();
        }

        var location = new Location(
            locationNameCreateResult.Value,
            addressCreateResult.Value,
            timezoneCreateResult.Value,
            []);

        var addResult = await repository.AddAsync(location, cancellationToken);

        if (addResult.IsFailure)
        {
            return addResult.Error.ToErrors();
        }

        logger.LogInformation("Создана локация с id = {locationId}", location.Id);

        return location.Id.Value;
    }
}