using CSharpFunctionalExtensions;
using FluentValidation;
using Primitives;
using Primitives.Abstractions;
using Primitives.Extensions;

namespace DirectoryService.Application.Decorators;

public class ValidationDecorator<TResponse, TCommand>(
    IEnumerable<IValidator<TCommand>> validators,
    ICommandHandler<TResponse, TCommand> inner)
    : ICommandHandler<TResponse, TCommand>
    where TCommand : IValidation
{
    public async Task<Result<TResponse, Failure>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await inner.Handle(command, cancellationToken);
        }

        var context = new ValidationContext<TCommand>(command);
        var validationResults = await Task.WhenAll(
            validators.Select(x => x.ValidateAsync(context, cancellationToken)));

        var results = validationResults
            .Where(x => !x.IsValid)
            .ToList();

        if (results.Count > 0)
        {
            return results.ToErrors();
        }

        var result = await inner.Handle(command, cancellationToken);

        return result;
    }
}