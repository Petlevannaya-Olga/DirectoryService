using DirectoryService.Contracts;
using DirectoryService.Contracts.Positions;
using DirectoryService.Domain.Positions;
using FluentValidation;
using Primitives;
using Primitives.Extensions;

namespace DirectoryService.Application.Positions.CreatePosition;

public class CreatePositionValidator : AbstractValidator<CreatePositionCommand>
{
    public CreatePositionValidator()
    {
        RuleFor(x => x.Dto.Name)
            .MustBeValueObject(PositionName.Create);

        RuleFor(x => x.Dto.Description)
            .MustBeValueObject(Description.Create);

        RuleFor(x => x.Dto.DepartmentIds)
            .NotEmpty()
            .WithError(CommonErrors
                .CollectionIsEmpty(nameof(CreatePositionDto.DepartmentIds), "Список подразделений не может быть пустым"))
            .MustBeUnique();
    }
}