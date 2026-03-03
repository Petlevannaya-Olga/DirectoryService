using CSharpFunctionalExtensions;
using FluentValidation;
using FluentValidation.Results;

namespace Primitives.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptionsConditions<T, TElement> MustBeValueObject<T, TElement, TValueObject>(
        this IRuleBuilder<T, TElement> ruleBuilder,
        Func<TElement, Result<TValueObject, Error>> factoryMethod)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            Result<TValueObject, Error> result = factoryMethod.Invoke(value);

            if (result.IsSuccess)
            {
                return;
            }

            context.AddFailure(new ValidationFailure(
                result.Error.InvalidField,
                result.Error.Message) { ErrorCode = result.Error.Code });
        });
    }

    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule, Error error)
    {
        return rule
            .WithMessage(error.Message)
            .WithErrorCode(error.Code);
    }
}