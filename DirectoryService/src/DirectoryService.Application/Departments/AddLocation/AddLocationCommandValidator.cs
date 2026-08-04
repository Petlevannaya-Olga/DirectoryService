using FluentValidation;

namespace DirectoryService.Application.Departments.AddLocation;

public sealed class AddLocationCommandValidator
    : AbstractValidator<AddLocationCommand>
{
    public AddLocationCommandValidator()
    {
        RuleFor(command => command.DepartmentId)
            .NotEmpty()
            .WithErrorCode("departmentId.is.empty")
            .WithMessage("Идентификатор подразделения обязателен");

        RuleFor(command => command.LocationId)
            .NotEmpty()
            .WithErrorCode("locationId.is.empty")
            .WithMessage("Идентификатор локации обязателен");
    }
}