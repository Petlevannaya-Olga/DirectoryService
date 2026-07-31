using FluentValidation;
using Primitives;
using Primitives.Extensions;

namespace DirectoryService.Application.Locations.UpdateLocations;

public class UpdateLocationsValidator : AbstractValidator<UpdateLocations.UpdateLocationsCommand>
{
    public UpdateLocationsValidator()
    {
        RuleFor(x => x.LocationIds)
            .NotEmpty()
            .WithError(CommonErrors.CollectionIsEmpty($"{nameof(UpdateLocations.UpdateLocationsCommand.LocationIds)} cannot be empty)"))
            .MustBeUnique();

        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .WithError(CommonErrors.IsRequired($"{nameof(UpdateLocations.UpdateLocationsCommand.DepartmentId)} cannot be empty"));
    }
}