using DirectoryService.Domain.Departments;
using FluentValidation;
using Primitives;
using Primitives.Extensions;

namespace DirectoryService.Application.Departments.UpdateDepartment;

public sealed class UpdateDepartmentCommandValidator
    : AbstractValidator<UpdateDepartmentCommand>
{
    public UpdateDepartmentCommandValidator()
    {
        RuleFor(command => command.DepartmentId)
            .NotEmpty()
            .WithError(CommonErrors.IsEmpty(nameof(DepartmentId)));

        RuleFor(x => x.Name)
            .MustBeValueObject(DepartmentName.Create);
    }
}