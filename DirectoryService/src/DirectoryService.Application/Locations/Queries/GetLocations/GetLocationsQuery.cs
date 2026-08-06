using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.Queries.GetLocations;

public sealed record GetLocationsQuery(
    string? Search,
    int? MinDepartmentCount,
    string? SortBy,
    string? SortDirection,
    int Page,
    int PageSize) : IQueryValidation;