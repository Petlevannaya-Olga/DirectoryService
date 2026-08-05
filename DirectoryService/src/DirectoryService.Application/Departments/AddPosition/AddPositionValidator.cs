using FluentValidation;

namespace DirectoryService.Application.Departments.AddPosition;

public class AddPositionValidator : AbstractValidator<AddPositionCommand>
{
    public AddPositionValidator()
    {
    }
}