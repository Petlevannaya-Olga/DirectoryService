using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.AddLocation;

public sealed record AddLocationCommand(
    Guid DepartmentId,
    Guid LocationId) : ICommandValidation;