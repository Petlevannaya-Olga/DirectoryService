using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.Queries.GetDepartmentById;

public sealed record GetDepartmentByIdQuery(Guid DepartmentId) : IQuery;