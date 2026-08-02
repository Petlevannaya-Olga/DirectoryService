using DirectoryService.Domain.Departments;
using FluentValidation;
using Primitives.Extensions;

namespace DirectoryService.Application.Departments.UpdateDepartment;

public sealed class UpdateDepartmentCommandValidator
    : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentCommandValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .WithMessage("Идентификатор подразделения не должен быть пустым");

        RuleFor(x => x.Name)
            .MustBeValueObject(DepartmentName.Create);
    }
}