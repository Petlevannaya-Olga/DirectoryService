using CSharpFunctionalExtensions;
using Primitives;

namespace DirectoryService.Domain.Departments;

public sealed class Path : ValueObject
{
    /// <summary>
    /// Разделитель
    /// </summary>
    private const char SEPARATOR = '/';

    /// <summary>
    /// Значение
    /// </summary>
    public string Value { get; private set; }

    private Path(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Путь к родительскому подразделению
    /// </summary>
    /// <param name="identifier">Идентификатор родительского подразделения</param>
    /// <returns>Новый путь</returns>
    public static Path CreateParent(Identifier identifier)
    {
        return new Path(identifier.Value);
    }

    /// <summary>
    /// Путь к дочернему подразделению
    /// </summary>
    /// <param name="identifier">Идентификатор дочернего подразделения</param>
    /// <returns>Новый путь</returns>
    public Path CreateChild(Identifier identifier)
    {
        return new Path(Value + SEPARATOR + identifier.Value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}