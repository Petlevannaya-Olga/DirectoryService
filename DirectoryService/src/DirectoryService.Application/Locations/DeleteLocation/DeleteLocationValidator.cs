using DirectoryService.Domain.Locations;
using FluentValidation;
using Primitives;
using Primitives.Extensions;

namespace DirectoryService.Application.Locations.DeleteLocation;

public class DeleteLocationValidator: AbstractValidator<DeleteLocationCommand>
{
    public DeleteLocationValidator()
    {
        RuleFor(command => command.LocationId)
            .NotEmpty()
            .WithError(CommonErrors.IsEmpty(nameof(LocationId)));
    }
}