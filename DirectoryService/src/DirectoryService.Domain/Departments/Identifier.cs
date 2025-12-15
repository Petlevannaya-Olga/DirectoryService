using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using Primitives;

namespace DirectoryService.Domain.Departments;

public partial class Identifier(string value) : ValueObject
{
    /// <summary>
    /// Минимальное значение длины строки
    /// </summary>
    private const int MIN_LENGTH = 3;
    
    /// <summary>
    /// Максимальное значение длины строки
    /// </summary>
    private const int MAX_LENGTH = 150;

    /// <summary>
    /// Значение
    /// </summary>
    public string Value { get; private set; } = value;

    /// <summary>
    /// Фабричный метод
    /// </summary>
    /// <param name="name">Название</param>
    /// <returns>Новый идентификатор</returns>
    public static Result<Identifier, Error> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return CommonErrors.ValueIsRequired(nameof(name));
        }

        if (name.Length is < MIN_LENGTH or > MAX_LENGTH)
        {
            return CommonErrors.ValueLengthIsWrong(nameof(name), MIN_LENGTH, MAX_LENGTH);
        }

        if (LatinRegex().IsMatch(name) is false)
        {
            return Errors.WrongIdentifierFormat(name);
        }

        return new Identifier(name.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    [GeneratedRegex(@"^[A-Za-z]+$")]
    private static partial Regex LatinRegex();

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
                $"Идентификатор {identifier} должен содержать только латинские символы");
        }
    }
}