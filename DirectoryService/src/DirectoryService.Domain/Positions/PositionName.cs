using CSharpFunctionalExtensions;
using Primitives;

namespace DirectoryService.Domain.Positions;

public class PositionName(string value) : ValueObject
{
    /// <summary>
    /// Минимальное значение длины строки
    /// </summary>
    private const int MIN_LENGTH = 3;

    /// <summary>
    /// Максимальное значение длины строки
    /// </summary>
    private const int MAX_LENGTH = 100;

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

        if (name.Length is < MIN_LENGTH or > MAX_LENGTH)
        {
            return CommonErrors.LengthIsWrong(nameof(name), MIN_LENGTH, MAX_LENGTH);
        }

        return new PositionName(name.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}