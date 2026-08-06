using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using FluentValidation;
using Primitives;
using Primitives.Extensions;

namespace DirectoryService.Application.Departments.RemoveLocation;

public sealed class RemoveLocationFromDepartmentCommandValidator
    : AbstractValidator<RemoveLocationCommand>
{
    public RemoveLocationFromDepartmentCommandValidator()
    {
        RuleFor(command => command.DepartmentId)
            .NotEmpty()
            .WithError(CommonErrors.IsEmpty(nameof(DepartmentId)));

        RuleFor(command => command.LocationId)
            .NotEmpty()
            .WithError(CommonErrors.IsEmpty(nameof(LocationId)));
    }
}