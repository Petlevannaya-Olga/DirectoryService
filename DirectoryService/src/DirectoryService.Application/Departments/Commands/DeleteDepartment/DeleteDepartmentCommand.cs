using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.DeleteDepartment;

public sealed record DeleteDepartmentCommand(Guid DepartmentId) : ICommandValidation;