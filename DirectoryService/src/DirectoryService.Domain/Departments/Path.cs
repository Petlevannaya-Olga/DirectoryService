using CSharpFunctionalExtensions;
using Primitives;

namespace DirectoryService.Domain.Departments;

public class Path(string value) : ValueObject
{
    /// <summary>
    /// Значение
    /// </summary>
    public string Value { get; private set; } = value;

    /// <summary>
    /// Фабричный метод
    /// </summary>
    /// <param name="name">Название</param>
    /// <returns>Новый путь</returns>
    public static Result<Path, Error> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return CommonErrors.ValueIsRequired(nameof(name));
        }

        return new Path(name.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}