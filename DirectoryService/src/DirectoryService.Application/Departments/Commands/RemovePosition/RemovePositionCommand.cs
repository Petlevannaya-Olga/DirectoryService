using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.RemovePosition;

public sealed record RemovePositionCommand(
    Guid DepartmentId,
    Guid PositionId) : ICommandValidation;