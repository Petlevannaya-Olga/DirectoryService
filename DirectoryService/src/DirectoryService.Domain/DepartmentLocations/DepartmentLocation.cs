using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;

namespace DirectoryService.Domain.DepartmentLocations;

/// <summary>
/// Связь многие-ко-многим между подразделениями и локациями
/// </summary>
public sealed class DepartmentLocation
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public DepartmentLocationId Id { get; } = null!;

    /// <summary>
    /// Идентификатор подразделения
    /// </summary>
    public DepartmentId DepartmentId { get; } = null!;

    /// <summary>
    /// Идентификатор локации
    /// </summary>
    public LocationId LocationId { get; } = null!;

    /// <summary>
    /// Конструктор с параметрами
    /// </summary>
    /// <param name="departmentId">Идентификатор подразделения</param>
    /// <param name="locationId">Идентификатор локации</param>
    internal DepartmentLocation(
        DepartmentId departmentId,
        LocationId locationId)
    {
        Id = new DepartmentLocationId(Guid.NewGuid());
        DepartmentId = departmentId;
        LocationId = locationId;
    }

    // EF Core
    private DepartmentLocation()
    {
    }
}