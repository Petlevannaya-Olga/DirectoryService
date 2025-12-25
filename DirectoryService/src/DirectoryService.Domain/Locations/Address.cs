using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Primitives;

namespace DirectoryService.Domain.Locations;

public class Address(
    string postalCode,
    string region,
    string city,
    string street,
    int house,
    int? apartment)
    : ValueObject
{
    /// <summary>
    /// Почтовый индекс
    /// </summary>
    public string PostalCode { get; private set; } = postalCode;

    /// <summary>
    /// Регион / субъект (область, край, штат)
    /// </summary>
    public string Region { get; private set; } = region;

    /// <summary>
    /// Город / населённый пункт
    /// </summary>
    public string City { get; private set; } = city;

    /// <summary>
    /// Улица
    /// </summary>
    public string Street { get; private set; } = street;

    /// <summary>
    /// Дом
    /// </summary>
    public int House { get; private set; } = house;

    /// <summary>
    /// Квартира / офис
    /// </summary>
    public int? Apartment { get; private set; } = apartment;

    /// <summary>
    /// Фабричный метод
    /// </summary>
    /// <param name="postalCode">Индекс</param>
    /// <param name="region">регион / субъект (область, край, штат)</param>
    /// <param name="city">город / населённый пункт</param>
    /// <param name="street">улица</param>
    /// <param name="house">дом</param>
    /// <param name="apartment">квартира / офис</param>
    /// <returns>Новый адрес</returns>
    public static Result<Address, Error> Create(
        string postalCode,
        string region,
        string city,
        string street,
        int house,
        int? apartment)
    {
        if (string.IsNullOrWhiteSpace(postalCode))
        {
            return CommonErrors.IsRequired(nameof(postalCode));
        }

        if (postalCode.Length != 6)
        {
            return Errors.WrongPostalCodeLength(postalCode);
        }

        if (string.IsNullOrWhiteSpace(region))
        {
            return CommonErrors.IsRequired(nameof(region));
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            return CommonErrors.IsRequired(nameof(city));
        }

        if (string.IsNullOrWhiteSpace(street))
        {
            return CommonErrors.IsRequired(nameof(street));
        }

        if (house < 1)
        {
            return Errors.WrongNumber(house);
        }

        if (apartment is < 1)
        {
            return Errors.WrongNumber(apartment.Value);
        }

        return new Address(postalCode, region, city, street, house, apartment);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PostalCode;
        yield return Region;
        yield return City;
        yield return Street;
        yield return House;
        if (Apartment != null)
        {
            yield return Apartment;
        }
    }

    /// <summary>
    /// Ошибки, которые может возвращать сущность
    /// </summary>
    [ExcludeFromCodeCoverage]
    private static class Errors
    {
        public static Error WrongPostalCodeLength(string postalCode)
        {
            return new Error(
                $"{postalCode}.is.wrong.postal.code.length",
                $"Неверно задан индекс: {postalCode}",
                ErrorType.VALIDATION);
        }

        public static Error WrongNumber(int number)
        {
            return new Error(
                $"{number}.is.wrong.number",
                $"Номер должен быть больше нуля:  {number}",
                ErrorType.VALIDATION);
        }
    }
}