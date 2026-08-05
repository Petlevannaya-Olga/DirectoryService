using DirectoryService.Contracts.Positions;
using DirectoryService.Domain.Positions;
using FluentValidation;
using Primitives;
using Primitives.Extensions;

namespace DirectoryService.Application.Positions.UpdatePositionName;

public class UpdatePositionNameValidator : AbstractValidator<UpdatePositionNameCommand>
{
    public UpdatePositionNameValidator()
    {
        RuleFor(command => command.PositionId)
            .NotEmpty()
            .WithError(CommonErrors.IsEmpty(nameof(PositionId)));

        RuleFor(x => x.Dto.Name)
            .MustBeValueObject(PositionName.Create);
    }
}