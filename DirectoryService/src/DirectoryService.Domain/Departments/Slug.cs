using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using Primitives;

namespace DirectoryService.Domain.Departments;

public sealed partial class Slug : ValueObject
{
    /// <summary>
    /// Минимальное значение длины строки
    /// </summary>
    public const int MINLENGTH = 3;

    /// <summary>
    /// Максимальное значение длины строки
    /// </summary>
    public const int MAXLENGTH = 150;

    /// <summary>
    /// Значение
    /// </summary>
    public string Value { get; private set; }

    private Slug(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Фабричный метод
    /// </summary>
    /// <param name="identifier">Название</param>
    /// <returns>Новый идентификатор</returns>
    public static Result<Slug, Error> Create(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return CommonErrors.IsRequired(nameof(identifier));
        }

        if (identifier.Length is < MINLENGTH or > MAXLENGTH)
        {
            return CommonErrors.LengthIsWrong(nameof(identifier), MINLENGTH, MAXLENGTH);
        }

        if (!LatinRegex.IsMatch(identifier))
        {
            return Errors.WrongSlugFormat(identifier);
        }

        return new Slug(identifier.Trim());
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
        public static Error WrongSlugFormat(string slug)
        {
            return new Error(
                $"{slug}.is.wrong.slug.format",
                $"{slug} должен содержать только латинские символы",
                ErrorType.VALIDATION,
                nameof(Slug));
        }
    }

    [GeneratedRegex(@"^[A-Za-z]+$", RegexOptions.Compiled, matchTimeoutMilliseconds: 1000)]
    private static partial Regex LatinRegex { get; }
}