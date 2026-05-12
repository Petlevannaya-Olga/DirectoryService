using FluentValidation;
using Primitives;
using Primitives.Extensions;

namespace DirectoryService.Application.Locations.UpdateLocationsCommand;

public class UpdateLocationsValidator : AbstractValidator<UpdateLocationsCommand>
{
    public UpdateLocationsValidator()
    {
        RuleFor(x => x.LocationIds)
            .NotEmpty()
            .WithError(CommonErrors.CollectionIsEmpty($"{nameof(UpdateLocationsCommand.LocationIds)} cannot be empty)"))
            .MustBeUnique();

        RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .WithError(CommonErrors.IsRequired($"{nameof(UpdateLocationsCommand.DepartmentId)} cannot be empty"));
    }
}