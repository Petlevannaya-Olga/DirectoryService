namespace DirectoryService.Contracts.Departments;

public sealed record GetDepartmentDto(
    Guid Id,
    string Name,
    string Slug,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
