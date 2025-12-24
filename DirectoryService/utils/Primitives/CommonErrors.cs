namespace Primitives;

public static class CommonErrors
{
    public static Error ValueIsRequired(string value) =>
        new($"{nameof(value).ToLowerInvariant()}.is.required", $"Значение не задано для {value}",
            ErrorType.VALIDATION);

    public static Error ValueLengthIsWrong(string value, int minLength, int maxLength)
        => new(
            $"{nameof(value).ToLowerInvariant()}.length.is.wrong",
            $"Значение должно быть длиной от {minLength} до {maxLength} символов для {value}",
            ErrorType.VALIDATION);

    public static Error None = new (string.Empty, string.Empty, ErrorType.NONE);

    public static Error NotFound(string? code, string message, Guid? id)
        => new(code ?? "record.not.found", message, ErrorType.NOT_FOUND);

    public static Error Validation(string? code, string message, string? invalidField = null)
        => new(code ?? "value.is.invalid", message, ErrorType.VALIDATION, invalidField);

    public static Error Conflict(string? code, string message)
        => new(code ?? "value.is.conflict", message, ErrorType.CONFLICT);

    public static Error Failure(string? code, string message)
        => new(code ?? "failure", message, ErrorType.FAILURE);
}