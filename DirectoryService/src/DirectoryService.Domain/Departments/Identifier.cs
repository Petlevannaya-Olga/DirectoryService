using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using Primitives;

namespace DirectoryService.Domain.Departments;

public sealed class Identifier : ValueObject
{
    /// <summary>
    /// Минимальное значение длины строки
    /// </summary>
    public const int MINLENGTH = 3;

    /// <summary>
    /// Максимальное значение длины строки
    /// </summary>
    public const int MAXLENGTH = 150;

    private static readonly Regex _latinRegex = new(@"^[A-Za-z]+$", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    /// <summary>
    /// Значение
    /// </summary>
    public string Value { get; private set; }

    private Identifier(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Фабричный метод
    /// </summary>
    /// <param name="identifier">Название</param>
    /// <returns>Новый идентификатор</returns>
    public static Result<Identifier, Error> Create(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return CommonErrors.IsRequired(nameof(identifier));
        }

        if (identifier.Length is < MINLENGTH or > MAXLENGTH)
        {
            return CommonErrors.LengthIsWrong(nameof(identifier), MINLENGTH, MAXLENGTH);
        }

        if (!_latinRegex.IsMatch(identifier))
        {
            return Errors.WrongIdentifierFormat(identifier);
        }

        return new Identifier(identifier.Trim());
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
        public static Error WrongIdentifierFormat(string identifier)
        {
            return new Error(
                $"{identifier}.is.wrong.identifier.format",
                $"Идентификатор {identifier} должен содержать только латинские символы",
                ErrorType.VALIDATION,
                nameof(Identifier));
        }
    }
}