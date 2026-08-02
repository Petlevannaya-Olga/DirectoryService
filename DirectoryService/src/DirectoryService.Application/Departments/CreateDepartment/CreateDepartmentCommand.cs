using DirectoryService.Contracts.Departments;
using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.CreateDepartment;

public record CreateDepartmentCommand(CreateDepartmentDto Dto) : IValidation;