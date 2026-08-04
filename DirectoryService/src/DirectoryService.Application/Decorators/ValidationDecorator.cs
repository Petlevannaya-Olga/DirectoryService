using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;
using Primitives.Extensions;

namespace DirectoryService.Application.Decorators;

public sealed class ValidationDecorator<TResponse, TCommand>(
    IEnumerable<IValidator<TCommand>> validators,
    ICommandHandler<TResponse, TCommand> inner,
    ILogger<ValidationDecorator<TResponse, TCommand>> logger)
    : ICommandHandler<TResponse, TCommand>
    where TCommand : IValidation
{
    public async Task<Result<TResponse, Errors>> Handle(
        TCommand command,
        CancellationToken cancellationToken)
    {
        var validatorsArray = validators.ToArray();

        if (validatorsArray.Length == 0)
        {
            return await inner.Handle(command, cancellationToken);
        }

        var context = new ValidationContext<TCommand>(command);
        var validationResults = new List<ValidationResult>();

        foreach (var validator in validatorsArray)
        {
            var validationResult = await validator.ValidateAsync(
                context,
                cancellationToken);

            validationResults.Add(validationResult);
        }

        var failedResults = validationResults
            .Where(result => !result.IsValid)
            .ToList();

        if (failedResults.Count == 0)
        {
            return await inner.Handle(command, cancellationToken);
        }

        var errors = failedResults.ToErrors();

        logger.LogWarning(
            "Команда {CommandType} не прошла валидацию: {@Errors}",
            typeof(TCommand).Name,
            errors);

        return errors;

    }
}