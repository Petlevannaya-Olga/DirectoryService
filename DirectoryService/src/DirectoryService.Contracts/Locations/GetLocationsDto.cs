namespace DirectoryService.Contracts.Locations;

public sealed class GetLocationsDto
{
    public string? Search { get; init; }

    public int? MinDepartmentCount { get; init; }

    public string? SortBy { get; init; } = "name";

    public string? SortDir { get; init; } = "asc";

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}