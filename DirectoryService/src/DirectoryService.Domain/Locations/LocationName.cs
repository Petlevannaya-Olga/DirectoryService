using CSharpFunctionalExtensions;
using Primitives;

namespace DirectoryService.Domain.Locations;

public class LocationName(string value) : ValueObject
{
    /// <summary>
    /// Минимальное значение длины строк
    /// </summary>
    private const int MIN_LENGTH = 3;

    /// <summary>
    /// Максимальное значение длины строки
    /// </summary>
    private const int MAX_LENGTH = 120;

    /// <summary>
    /// Значение
    /// </summary>
    public string Value { get; private set; } = value;

    /// <summary>
    /// Фабричный метод
    /// </summary>
    /// <param name="name">Название</param>
    /// <returns>Новое название локации</returns>
    public static Result<LocationName, Error> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return CommonErrors.IsRequired(nameof(name));
        }

        if (name.Length is < MIN_LENGTH or > MAX_LENGTH)
        {
            return CommonErrors.LengthIsWrong(nameof(name), MIN_LENGTH, MAX_LENGTH);
        }

        return new LocationName(name.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}