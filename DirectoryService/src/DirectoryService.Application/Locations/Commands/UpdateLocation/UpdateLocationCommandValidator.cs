using DirectoryService.Domain.Locations;
using FluentValidation;
using Primitives;
using Primitives.Extensions;

namespace DirectoryService.Application.Locations.Commands.UpdateLocation;

public sealed class UpdateLocationCommandValidator
    : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty()
            .WithError(CommonErrors.IsEmpty(nameof(LocationId)));

        RuleFor(c => c.Name)
            .MustBeValueObject(LocationName.Create);

        RuleFor(c => c.LocationAddress)
            .MustBeValueObject(Address.Create);

        RuleFor(command => command.Timezone)
            .MustBeValueObject(Timezone.Create);
    }
}