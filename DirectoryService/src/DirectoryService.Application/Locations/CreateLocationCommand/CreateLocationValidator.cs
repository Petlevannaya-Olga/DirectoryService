using DirectoryService.Domain.Locations;
using FluentValidation;
using Primitives.Extensions;

namespace DirectoryService.Application.Locations.CreateLocationCommand;

public class CreateLocationValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationValidator()
    {
        RuleFor(x => x.Dto.Name)
            .MustBeValueObject(LocationName.Create);

        RuleFor(x => x.Dto.Timezone)
            .MustBeValueObject(Timezone.Create);

        RuleFor(x => x.Dto.Address)
            .MustBeValueObject(Address.Create);
    }
}