using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;
using FluentValidation;
using Primitives;
using Primitives.Extensions;

namespace DirectoryService.Application.Departments.AddPosition;

public class AddPositionValidator : AbstractValidator<AddPositionCommand>
{
    public AddPositionValidator()
    {
        RuleFor(command => command.DepartmentId)
            .NotEmpty()
            .WithError(CommonErrors.IsEmpty(nameof(DepartmentId)));

        RuleFor(command => command.PositionId)
            .NotEmpty()
            .WithError(CommonErrors.IsEmpty(nameof(PositionId)));
    }
}