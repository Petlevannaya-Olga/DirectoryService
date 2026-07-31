namespace DirectoryService.Contracts;

public record UpdateLocationDto(Guid Id, string Name, string Description, Guid[] DepartmentIds);