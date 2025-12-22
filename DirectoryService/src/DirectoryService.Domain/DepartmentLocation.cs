using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Domain;

/// <summary>
/// Связь многие-ко-многим между подразделениями и локациями
/// </summary>
public class DepartmentLocation
{
    /// <summary>
    /// Идентификатор подразделения
    /// </summary>
    public Guid DepartmentId { get; private set; }

    /// <summary>
    /// Подразделение, навигационное свойство
    /// </summary>
    public Department Department { get; private set; } = null!;

    /// <summary>
    /// Идентификатор локации
    /// </summary>
    public Guid LocationId { get; private set; }

    /// <summary>
    /// Локация, навигационное свойство
    /// </summary>
    public Location Location { get; private set; } = null!;

    /// <summary>
    /// Конструктор с параметрами
    /// </summary>
    /// <param name="departmentId">Идентификатор подразделения</param>
    /// <param name="locationId">Идентификатор локации</param>
    public DepartmentLocation(Guid departmentId, Guid locationId)
    {
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    // EF Core
    private DepartmentLocation() { }
}