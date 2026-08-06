namespace DirectoryService.Contracts.Locations;

public sealed record GetLocationDto(
    Guid Id,
    string Name,
    LocationAddressDto Address,
    string Timezone,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);