using DirectoryService.Contracts.Locations;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.UpdateLocation;

public sealed record UpdateLocationCommand(
    Guid Id,
    string Name,
    AddressDto Address,
    string Timezone) : IValidation;