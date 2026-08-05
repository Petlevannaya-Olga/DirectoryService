using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;
using FluentValidation;
using Primitives;
using Primitives.Extensions;

namespace DirectoryService.Application.Departments.RemovePosition;

public class RemovePositionValidator : AbstractValidator<RemovePositionCommand>
{
    public RemovePositionValidator()
    {
        RuleFor(command => command.DepartmentId)
            .NotEmpty()
            .WithError(CommonErrors.IsEmpty(nameof(DepartmentId)));

        RuleFor(command => command.PositionId)
            .NotEmpty()
            .WithError(CommonErrors.IsEmpty(nameof(PositionId)));
    }
}