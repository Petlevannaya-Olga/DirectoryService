using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Locations;
using Primitives;

namespace DirectoryService.Domain.Locations;

public sealed class Address(
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
    public string PostalCode { get; } = postalCode;

    /// <summary>
    /// Регион / субъект (область, край, штат)
    /// </summary>
    public string Region { get; } = region;

    /// <summary>
    /// Город / населённый пункт
    /// </summary>
    public string City { get; } = city;

    /// <summary>
    /// Улица
    /// </summary>
    public string Street { get; } = street;

    /// <summary>
    /// Дом
    /// </summary>
    public int House { get; } = house;

    /// <summary>
    /// Квартира / офис
    /// </summary>
    public int? Apartment { get; } = apartment;

    /// <summary>
    /// Фабричный метод
    /// </summary>
    /// <param name="dto">Address dto</param>
    /// <returns>Новый адрес</returns>
    public static Result<Address, Error> Create(LocationAddressDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.PostalCode))
        {
            return CommonErrors.IsRequired(nameof(dto.PostalCode));
        }

        if (dto.PostalCode.Length != 6)
        {
            return Errors.WrongPostalCodeLength(dto.PostalCode);
        }

        if (string.IsNullOrWhiteSpace(dto.Region))
        {
            return CommonErrors.IsRequired(nameof(dto.Region));
        }

        if (string.IsNullOrWhiteSpace(dto.City))
        {
            return CommonErrors.IsRequired(nameof(dto.City));
        }

        if (string.IsNullOrWhiteSpace(dto.Street))
        {
            return CommonErrors.IsRequired(nameof(dto.Street));
        }

        if (dto.House < 1)
        {
            return Errors.WrongNumber(dto.House, nameof(dto.House));
        }

        if (dto.Apartment is < 1)
        {
            return Errors.WrongNumber(dto.Apartment.Value, nameof(dto.Apartment));
        }

        return new Address(
            dto.PostalCode,
            dto.Region,
            dto.City,
            dto.Street,
            dto.House,
            dto.Apartment);
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
                ErrorType.VALIDATION,
                nameof(PostalCode));
        }

        public static Error WrongNumber(int number, string invalidField)
        {
            return new Error(
                $"{number}.is.wrong.number",
                $"Номер должен быть больше нуля:  {number}",
                ErrorType.VALIDATION,
                invalidField);
        }
    }
}