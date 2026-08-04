using DirectoryService.Contracts;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Departments;
using FluentValidation;
using Primitives;
using Primitives.Extensions;

namespace DirectoryService.Application.Departments.CreateDepartment;

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentValidator()
    {
        RuleFor(x => x.Dto.Name)
            .MustBeValueObject(DepartmentName.Create);

        RuleFor(x => x.Dto.Slug)
            .MustBeValueObject(Slug.Create);

        RuleFor(x => x.Dto.LocationIds)
            .NotEmpty()
            .WithError(CommonErrors.CollectionIsEmpty($"{nameof(CreateDepartmentDto.LocationIds)} не может быть пустым"))
            .MustBeUnique();
    }
}