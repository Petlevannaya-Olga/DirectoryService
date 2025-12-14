namespace Primitives;

public static class CommonErrors
{
    public static Error ValueIsRequired(string value) =>
        new Error($"{nameof(value).ToLowerInvariant()}.is.required", $"Значение не задано для {value}");

    public static Error ValueLengthIsWrong(string value, int minLength, int maxLength)
    {
        return new Error(
            $"{nameof(value).ToLowerInvariant()}.length.is.wrong",
            $"Значение должно быть длиной от {minLength} до {maxLength} символов для {value}");
    }
}