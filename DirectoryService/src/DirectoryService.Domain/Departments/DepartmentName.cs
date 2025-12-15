using CSharpFunctionalExtensions;
using Primitives;

namespace DirectoryService.Domain.Departments;

public class DepartmentName(string value) : ValueObject
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
    /// <returns>Новое название подразделения</returns>
    public static Result<DepartmentName, Error> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return CommonErrors.ValueIsRequired(nameof(name));
        }

        if (name.Length is < MIN_LENGTH or > MAX_LENGTH)
        {
            return CommonErrors.ValueLengthIsWrong(nameof(name), MIN_LENGTH, MAX_LENGTH);
        }

        return new DepartmentName(name.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}