using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Locations;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.CreateLocationCommand;

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

        if (await repository.GetByAddressAsync(address.Value, cancellationToken) != null)
        {
            return CreateLocationErrors.LocationAddressConflict().ToErrors();
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
    private static class CreateLocationErrors
    {
        public static Error LocationAddressConflict() =>
            CommonErrors.Conflict(
                "location.address.conflict",
                $"Локация с таким адресом уже существует");
    }
}