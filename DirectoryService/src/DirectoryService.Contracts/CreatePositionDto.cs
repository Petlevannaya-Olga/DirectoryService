namespace DirectoryService.Contracts;

public record CreatePositionDto(string Name, string Description, Guid[] DepartmentIds);