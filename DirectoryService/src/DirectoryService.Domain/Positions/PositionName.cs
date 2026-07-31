using CSharpFunctionalExtensions;
using Primitives;

namespace DirectoryService.Domain.Positions;

public sealed class PositionName(string value) : ValueObject
{
    /// <summary>
    /// Минимальное значение длины строки
    /// </summary>
    public const int MINLENGTH = 3;

    /// <summary>
    /// Максимальное значение длины строки
    /// </summary>
    public const int MAXLENGTH = 100;

    /// <summary>
    /// Значение
    /// </summary>
    public string Value { get; private set; } = value;

    /// <summary>
    /// Фабричный метод
    /// </summary>
    /// <param name="name">Название</param>
    /// <returns>Новое название позиции</returns>
    public static Result<PositionName, Error> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return CommonErrors.IsRequired(nameof(name));
        }

        if (name.Length is < MINLENGTH or > MAXLENGTH)
        {
            return CommonErrors.LengthIsWrong(nameof(name), MINLENGTH, MAXLENGTH);
        }

        return new PositionName(name.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}