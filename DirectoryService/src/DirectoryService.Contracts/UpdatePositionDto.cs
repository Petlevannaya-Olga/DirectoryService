namespace DirectoryService.Contracts;

public record UpdatePositionDto(Guid Id, string Name, string Description, Guid[] DepartmentIds);