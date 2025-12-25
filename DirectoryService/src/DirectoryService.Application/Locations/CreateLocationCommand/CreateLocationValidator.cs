using FluentValidation;
using Primitives;
using Primitives.Extensions;

namespace DirectoryService.Application.Locations.CreateLocationCommand;

public class CreateLocationValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationValidator()
    {
        RuleFor(x => x.Dto.Name)
            .NotEmpty()
            .WithError(CommonErrors.IsRequired("name"))
            .MinimumLength(3)
            .WithError(CommonErrors.LengthIsTooShort("name", 3))
            .MaximumLength(150)
            .WithError(CommonErrors.LengthIsTooLarge("name", 150));

        RuleFor(x => x.Dto.Timezone)
            .NotEmpty()
            .WithError(CommonErrors.IsRequired("timezone"));

        RuleFor(x => x.Dto.Address)
            .NotNull()
            .WithError(CommonErrors.IsRequired("address"));

        RuleFor(x => x.Dto.Address.PostalCode)
            .NotEmpty()
            .WithError(CommonErrors.IsRequired("postalCode"))
            .Length(6)
            .WithError(CommonErrors.Validation(
                "postal.code.length.is.invalid",
                "Длина индекса должна быть равна 6",
                "postalCode"));

        RuleFor(x => x.Dto.Address.Region)
            .NotEmpty()
            .WithError(CommonErrors.IsRequired("region"));

        RuleFor(x => x.Dto.Address.City)
            .NotEmpty()
            .WithError(CommonErrors.IsRequired("city"));

        RuleFor(x => x.Dto.Address.Street)
            .NotEmpty()
            .WithError(CommonErrors.IsRequired("street"));

        RuleFor(x => x.Dto.Address.House)
            .GreaterThan(0)
            .WithError(CommonErrors.MustBePositive("house"));
    }
}