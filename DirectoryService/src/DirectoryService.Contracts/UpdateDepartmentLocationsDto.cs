namespace DirectoryService.Contracts;

public record UpdateDepartmentLocationsDto(IEnumerable<Guid> LocationIds);