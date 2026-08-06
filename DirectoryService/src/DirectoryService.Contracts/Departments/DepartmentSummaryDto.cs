namespace DirectoryService.Contracts.Departments;

public sealed record DepartmentSummaryDto(
    Guid Id,
    string Name,
    string Path,
    DateTime CreatedAt);