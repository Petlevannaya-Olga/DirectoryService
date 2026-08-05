namespace DirectoryService.Contracts.Departments;

public record CreateDepartmentDto(
    string Name,
    string Slug,
    Guid? ParentId,
    IEnumerable<Guid> LocationIds);