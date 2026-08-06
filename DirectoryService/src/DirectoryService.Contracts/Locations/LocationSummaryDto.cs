namespace DirectoryService.Contracts.Locations;

public sealed record LocationSummaryDto(
    Guid Id,
    string Name,
    string Address,
    DateTime CreatedAt,
    int DepartmentCount);