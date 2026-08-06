using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Locations;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.Queries.GetLocationById;

public sealed class GetLocationByIdHandler(
    IReadDbContext readDbContext)
    : IQueryHandler<
        GetLocationDto,
        GetLocationByIdQuery>
{
    public async Task<Result<GetLocationDto, Errors>> Handle(
        GetLocationByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var locationId =
            new LocationId(query.LocationId);

        var location = await readDbContext
            .LocationsRead
            .Where(item =>
                item.Id == locationId &&
                item.IsActive)
            .Select(item => new GetLocationDto(
                item.Id.Value,
                item.Name.Value,
                new LocationAddressDto(
                    item.Address.PostalCode,
                    item.Address.Region,
                    item.Address.City,
                    item.Address.Street,
                    item.Address.House,
                    item.Address.Apartment),
                item.Timezone.Value,
                item.IsActive,
                item.CreatedAt,
                item.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        if (location is null)
        {
            return CommonErrors
                .NotFound(
                    "location.not.found",
                    $"Локация с идентификатором '{query.LocationId}' не найдена",
                    query.LocationId)
                .ToErrors();
        }

        return location;
    }
}