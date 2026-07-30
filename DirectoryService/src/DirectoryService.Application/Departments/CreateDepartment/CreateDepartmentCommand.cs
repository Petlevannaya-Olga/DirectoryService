using DirectoryService.Contracts;
using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.CreateDepartment;

public record CreateDepartmentCommand(CreateDepartmentDto Dto) : IValidation;