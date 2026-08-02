namespace DirectoryService.Contracts.Departments;

public record UpdateDepartmentLocationsDto(IEnumerable<Guid> LocationIds);