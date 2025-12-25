using FluentValidation;

namespace DirectoryService.Application.Locations.CreateLocationCommand;

public class CreateLocationValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationValidator()
    {
        RuleFor(x => x.Dto.Name)
            .NotEmpty()
            .WithMessage("Название не может быть пустым")
            .MinimumLength(3)
            .WithMessage("Название должно быть длиной не менее 3-х символов")
            .MaximumLength(150)
            .WithMessage("Название не должно превышать 150 символов");

        RuleFor(x => x.Dto.Timezone)
            .NotEmpty()
            .WithMessage("Временная зона не может быть пустой");

        RuleFor(x => x.Dto.Address)
            .NotNull()
            .WithMessage("Адрес должен быть указан");

        RuleFor(x => x.Dto.Address.PostalCode)
            .NotEmpty()
            .WithMessage("Индекс не может быть пустым")
            .Length(6)
            .WithMessage("Длина индекса должна быть равна 6");

        RuleFor(x => x.Dto.Address.Region)
            .NotEmpty()
            .WithMessage("Регион не  может быть пустым");

        RuleFor(x => x.Dto.Address.City)
            .NotEmpty()
            .WithMessage("Город не может быть пустым");

        RuleFor(x => x.Dto.Address.Street)
            .NotEmpty()
            .WithMessage("Улица не может быть пустой");

        RuleFor(x => x.Dto.Address.House)
            .GreaterThan(0)
            .WithMessage("Номер дома должен быть положительным числом");
    }
}