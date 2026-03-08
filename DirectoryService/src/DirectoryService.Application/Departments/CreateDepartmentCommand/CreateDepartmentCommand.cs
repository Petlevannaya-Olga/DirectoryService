using DirectoryService.Contracts;
using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.CreateDepartmentCommand;

public record CreateDepartmentCommand(CreateDepartmentDto Dto) : IValidation;