using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.Queries.GetDepartments;

public sealed record GetDepartmentsQuery(
    string? Search,
    string? SortBy,
    string? SortDirection,
    int Page,
    int PageSize) : IQueryValidation;