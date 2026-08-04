using CSharpFunctionalExtensions;
using Primitives;

namespace DirectoryService.Domain.Locations;

public sealed class LocationName(string value) : ValueObject
{
    /// <summary>
    /// Минимальное значение длины строк
    /// </summary>
    public const int MINLENGTH = 3;

    /// <summary>
    /// Максимальное значение длины строки
    /// </summary>
    public const int MAXLENGTH = 120;

    /// <summary>
    /// Значение
    /// </summary>
    public string Value { get; } = value;

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

        if (name.Length is < MINLENGTH or > MAXLENGTH)
        {
            return CommonErrors.LengthIsWrong(nameof(name), MINLENGTH, MAXLENGTH);
        }

        return new LocationName(name.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}