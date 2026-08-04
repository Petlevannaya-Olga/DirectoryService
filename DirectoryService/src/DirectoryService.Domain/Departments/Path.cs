using CSharpFunctionalExtensions;

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
    public string Value { get; }

    private Path(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Путь к родительскому подразделению
    /// </summary>
    /// <param name="slug">Идентификатор родительского подразделения</param>
    /// <returns>Новый путь</returns>
    public static Path CreateParent(Slug slug)
    {
        return new Path(slug.Value);
    }

    /// <summary>
    /// Путь к дочернему подразделению
    /// </summary>
    /// <param name="slug">Идентификатор дочернего подразделения</param>
    /// <returns>Новый путь</returns>
    public Path CreateChild(Slug slug)
    {
        return new Path(Value + SEPARATOR + slug.Value);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}