using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.RemoveLocation;

public sealed record RemoveLocationCommand(
    Guid DepartmentId,
    Guid LocationId) : IValidation;