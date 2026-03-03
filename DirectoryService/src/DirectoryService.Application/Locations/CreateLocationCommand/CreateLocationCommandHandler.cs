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
            var errors = locationNameCreateResult.Error.ToErrors();
            logger.LogError("Ошибка валидации названия локации: {@Errors}", errors);
            return errors;
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
            var errors = addressCreateResult.Error.ToErrors();
            logger.LogError("Ошибка валидации адреса: {@Errors}", errors);
            return errors;
        }

        var timezoneCreateResult = Timezone.Create(command.Dto.Timezone);

        if (timezoneCreateResult.IsFailure)
        {
            var errors = timezoneCreateResult.Error.ToErrors();
            logger.LogError("Ошибка валидации временной зоны: {@Errors}", errors);
            return errors;
        }

        var location = new Location(
            locationNameCreateResult.Value,
            addressCreateResult.Value,
            timezoneCreateResult.Value,
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
}