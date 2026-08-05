using DirectoryService.Domain.DepartmentLocations;

namespace DirectoryService.Domain.Locations;

public sealed class Location
{
    private readonly List<DepartmentLocation> _departmentLocations = [];

    /// <summary>
    /// Идентификатор, PK
    /// </summary>
    public LocationId Id { get; private set; } = null!;

    /// <summary>
    /// Название, UNIQUE, 3–120 симв.
    /// </summary>
    public LocationName Name { get; private set; } = null!;

    /// <summary>
    /// Адрес, в БД может быть несколько столбцов или jsonb
    /// </summary>
    public Address Address { get; private set; } = null!;

    /// <summary>
    /// Код временной зоны, IANA
    /// </summary>
    public Timezone Timezone { get; private set; } = null!;

    /// <summary>
    /// Список подразделений
    /// </summary>
    public IReadOnlyList<DepartmentLocation> DepartmentLocations =>
        _departmentLocations;

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
    /// Конструктор с параметрами
    /// </summary>
    /// <param name="name">Название</param>
    /// <param name="address">Адрес</param>
    /// <param name="timezone">Код временной зоны</param>
    public Location(
        LocationName name,
        Address address,
        Timezone timezone)
    {
        var now = DateTime.UtcNow;

        Id = new LocationId(Guid.NewGuid());
        Name = name;
        Address = address;
        Timezone = timezone;
        IsActive = true;
        CreatedAt = now;
        UpdatedAt = now;
    }

    /// <summary>
    /// Конструктор без параметров, EF Core
    /// </summary>
    private Location()
    {
    }

    /// <summary>
    /// Обновляет редактируемые данные локации.
    /// </summary>
    /// <param name="name">Название локации.</param>
    /// <param name="address">Адрес локации.</param>
    /// <param name="timezone">Временная зона.</param>
    public void Update(
        LocationName name,
        Address address,
        Timezone timezone)
    {
        Name = name;
        Address = address;
        Timezone = timezone;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}