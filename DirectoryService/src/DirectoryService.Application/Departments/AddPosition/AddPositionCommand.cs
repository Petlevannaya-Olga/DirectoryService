using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.AddPosition;

public sealed record AddPositionCommand(
    Guid DepartmentId,
    Guid PositionId) : IValidation;