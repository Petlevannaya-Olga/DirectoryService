using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.UpdateDepartment;

public sealed record UpdateDepartmentCommand(
    Guid DepartmentId,
    string Name) : ICommandValidation;