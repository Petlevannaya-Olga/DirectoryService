using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Primitives;
using TimeZoneConverter;

namespace DirectoryService.Domain.Locations;

public sealed class Timezone(string value) : ValueObject
{
    /// <summary>
    /// Значение
    /// </summary>
    public string Value { get; private set; } = value;

    /// <summary>
    /// Фабричный метод
    /// </summary>
    /// <param name="name">Название</param>
    /// <returns>Новый код временной зоны</returns>
    public static Result<Timezone, Error> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return CommonErrors.IsRequired(nameof(name));
        }

        if (TZConvert.KnownIanaTimeZoneNames.Contains(name) is false)
        {
            return Errors.WrongTimezoneFormat(name);
        }

        return new Timezone(name.Trim());
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
        public static Error WrongTimezoneFormat(string timezone)
        {
            return new Error(
                $"{timezone}.is.wrong.timezone.format",
                $"Неверно задан код временной зоны: {timezone}",
                ErrorType.VALIDATION,
                nameof(Value));
        }
    }
}