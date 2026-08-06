namespace DirectoryService.Contracts.Locations;

public sealed record UpdateLocationDto(string Name, LocationAddressDto LocationAddress, string Timezone);