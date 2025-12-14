#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

using CSharpFunctionalExtensions;
using Primitives;

namespace DirectoryService.Domain.Locations;

public class Location
{
    private readonly List<DepartmentLocation> _departments = [];

    /// <summary>
    /// Идентификатор, PK
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Название, UNIQUE, 3–120 симв.
    /// </summary>
    public LocationName Name { get; private set; }

    /// <summary>
    /// Адрес, В бд может быть несколько столбцов или jsonb
    /// </summary>
    public Address Address { get; private set; }

    /// <summary>
    /// Код временной зоны, IANA
    /// </summary>
    public Timezone Timezone { get; private set; }

    /// <summary>
    /// Список департаментов
    /// </summary>
    public IReadOnlyList<DepartmentLocation> Departments => _departments;

    /// <summary>
    /// Для soft delete
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Дата создания
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Дата обновления
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Конструктор без параметров, EF Core
    /// </summary>
    private Location() { }

    /// <summary>
    /// Конструктор с параметрами
    /// </summary>
    /// <param name="name">Название</param>
    /// <param name="address">Адрес</param>
    /// <param name="timezone">Код временной зоны</param>
    /// <param name="departments">Список подразделений</param>
    private Location(
        LocationName name,
        Address address,
        Timezone timezone,
        IEnumerable<DepartmentLocation> departments)
    {
        Id = Guid.NewGuid();
        Name = name;
        Address = address;
        Timezone = timezone;
        IsActive = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        _departments = departments.ToList();
    }

    /// <summary>
    /// Фабричный метод
    /// </summary>
    /// <param name="name">Название</param>
    /// <param name="address">Адрес</param>
    /// <param name="timezone">Код временной зоны</param>
    /// <param name="departments">Список подразделений</param>
    /// <returns>Новая локация</returns>
    public static Result<Location, Error> Create(
        LocationName name,
        Address address,
        Timezone timezone,
        IEnumerable<DepartmentLocation> departments)
    {
        return new Location(name, address, timezone, departments);
    }
}