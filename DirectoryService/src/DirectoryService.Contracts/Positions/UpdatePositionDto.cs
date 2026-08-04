namespace DirectoryService.Contracts.Positions;

public record UpdatePositionDto(Guid Id, string Name, string Description, Guid[] DepartmentIds);