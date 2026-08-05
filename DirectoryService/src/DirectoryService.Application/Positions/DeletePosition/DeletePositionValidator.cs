using DirectoryService.Domain.Positions;
using FluentValidation;
using Primitives;
using Primitives.Extensions;

namespace DirectoryService.Application.Positions.DeletePosition;

public class DeletePositionValidator : AbstractValidator<DeletePositionCommand>
{
    public DeletePositionValidator()
    {
        RuleFor(command => command.PositionId)
            .NotEmpty()
            .WithError(CommonErrors.IsEmpty(nameof(PositionId)));
    }
}