using CSharpFunctionalExtensions;
using Primitives;

namespace DirectoryService.Domain.Locations;

public class Address(string value) : ValueObject
{
    /// <summary>
    /// Значение
    /// </summary>
    public string Value { get; private set; } = value;

    /// <summary>
    /// Фабричный метод
    /// </summary>
    /// <param name="name">Название</param>
    /// <returns>Новый адрес</returns>
    public static Result<Address, Error> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return CommonErrors.ValueIsRequired(nameof(name));
        }

        return new Address(name.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}