using DirectoryService.Contracts.Locations;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.Commands.UpdateLocation;

public sealed record UpdateLocationCommand(
    Guid Id,
    string Name,
    LocationAddressDto LocationAddress,
    string Timezone) : IValidation;