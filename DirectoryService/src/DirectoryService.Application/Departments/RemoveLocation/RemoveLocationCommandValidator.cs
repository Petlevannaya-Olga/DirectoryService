using FluentValidation;

namespace DirectoryService.Application.Departments.RemoveLocation;

public sealed class RemoveLocationFromDepartmentCommandValidator
    : AbstractValidator<RemoveLocationCommand>
{
    public RemoveLocationFromDepartmentCommandValidator()
    {
        RuleFor(command => command.DepartmentId)
            .NotEmpty()
            .WithMessage("Идентификатор подразделения обязателен");

        RuleFor(command => command.LocationId)
            .NotEmpty()
            .WithMessage("Идентификатор локации обязателен");
    }
}