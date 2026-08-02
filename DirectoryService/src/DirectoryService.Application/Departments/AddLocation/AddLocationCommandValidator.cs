using FluentValidation;

namespace DirectoryService.Application.Departments.AddLocation;

public sealed class AddLocationCommandValidator
    : AbstractValidator<AddLocationCommand>
{
    public AddLocationCommandValidator()
    {
        RuleFor(command => command.DepartmentId)
            .NotEmpty()
            .WithMessage("Идентификатор подразделения обязателен");

        RuleFor(command => command.LocationId)
            .NotEmpty()
            .WithMessage("Идентификатор локации обязателен");
    }
}