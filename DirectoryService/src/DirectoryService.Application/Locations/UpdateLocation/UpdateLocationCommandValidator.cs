using DirectoryService.Domain.Locations;
using FluentValidation;
using Primitives.Extensions;

namespace DirectoryService.Application.Locations.UpdateLocation;

public sealed class UpdateLocationCommandValidator
    : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
            .WithMessage("Идентификатор локации обязателен");

        RuleFor(c => c.Name)
            .MustBeValueObject(LocationName.Create);
           
        RuleFor(c => c.Address)
            .MustBeValueObject(Address.Create);

        RuleFor(command => command.Timezone)
           .MustBeValueObject(Timezone.Create);
    }
}