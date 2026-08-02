namespace DirectoryService.Contracts.Locations;

public sealed record UpdateLocationDto(string Name, AddressDto Address, string Timezone);