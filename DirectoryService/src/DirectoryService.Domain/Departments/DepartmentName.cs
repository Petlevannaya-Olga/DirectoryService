using CSharpFunctionalExtensions;
using Primitives;

namespace DirectoryService.Domain.Departments;

public sealed class DepartmentName : ValueObject
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
    public string Value { get; }

    private DepartmentName(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Фабричный метод
    /// </summary>
    /// <param name="name">Название</param>
    /// <returns>Новое название подразделения</returns>
    public static Result<DepartmentName, Error> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return CommonErrors.IsRequired(nameof(name));
        }

        if (name.Length is < MINLENGTH or > MAXLENGTH)
        {
            return CommonErrors.LengthIsWrong(nameof(name), MINLENGTH, MAXLENGTH);
        }

        return new DepartmentName(name.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}