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
    private const int MIN_LENGTH = 3;

    /// <summary>
    /// Максимальное значение длины строки
    /// </summary>
    private const int MAX_LENGTH = 150;

    private static readonly Regex _latinRegex = new(@"^[A-Za-z]+$", RegexOptions.Compiled);

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
    /// <param name="name">Название</param>
    /// <returns>Новый идентификатор</returns>
    public static Result<Identifier, Error> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return CommonErrors.IsRequired(nameof(name));
        }

        if (name.Length is < MIN_LENGTH or > MAX_LENGTH)
        {
            return CommonErrors.LengthIsWrong(nameof(name), MIN_LENGTH, MAX_LENGTH);
        }

        if (!_latinRegex.IsMatch(name))
        {
            return Errors.WrongIdentifierFormat(name);
        }

        return new Identifier(name.Trim());
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