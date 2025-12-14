using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Primitives;

namespace DirectoryService.Domain.Positions;

public class Description(string value) : ValueObject
{
    /// <summary>
    /// Максимальное значение длины строки
    /// </summary>
    private const int MAX_LENGTH = 1000;

    /// <summary>
    /// Значение
    /// </summary>
    public string Value { get; private set; } = value;

    /// <summary>
    /// Фабричный метод
    /// </summary>
    /// <param name="name">Название</param>
    /// <returns>Новое описание</returns>
    public static Result<Description, Error> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return CommonErrors.ValueIsRequired(nameof(name));
        }

        if (name.Length >= MAX_LENGTH)
        {
            return Errors.ValueLengthIsTooLarge();
        }

        return new Description(name.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Ошибки, которые может возвращать сущность
    /// </summary>
    [ExcludeFromCodeCoverage]
    private static class Errors
    {
        public static Error ValueLengthIsTooLarge()
        {
            return new Error(
                $"{nameof(Value).ToLowerInvariant()}.is.too.large",
                $"Длина описания превышает {MAX_LENGTH} символов");
        }
    }
}