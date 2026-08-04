using DirectoryService.Domain.Departments;
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
            .WithError(CommonErrors
                .CollectionIsEmpty(nameof(UpdateLocationsCommand.LocationIds), "Список локаций не может быть пустым"))
            .MustBeUnique();

        RuleFor(command => command.DepartmentId)
            .NotEmpty()
            .WithError(CommonErrors.IsEmpty(nameof(DepartmentId)));
    }
}