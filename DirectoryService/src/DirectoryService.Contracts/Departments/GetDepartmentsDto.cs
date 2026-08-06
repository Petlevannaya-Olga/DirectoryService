namespace DirectoryService.Contracts.Departments;

public sealed class GetDepartmentsDto
{
    public string? Search { get; init; }

    public string? SortBy { get; init; } = "name";

    public string? SortDirection { get; init; } = "asc";

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}