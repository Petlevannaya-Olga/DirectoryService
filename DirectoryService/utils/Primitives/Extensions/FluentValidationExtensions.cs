using FluentValidation;

namespace Primitives.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> ruleBuilder,
        Error error)
    {
        return ruleBuilder
            .WithMessage(error.Message)
            .WithErrorCode(error.Code);
    }
}