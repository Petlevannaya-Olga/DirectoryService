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
    public DepartmentLocationsId Id { get; private set; }

    /// <summary>
    /// Идентификатор подразделения
    /// </summary>
    public DepartmentId DepartmentId { get; private set; }

    /// <summary>
    /// Идентификатор локации
    /// </summary>
    public LocationId LocationId { get; private set; }

    /// <summary>
    /// Конструктор с параметрами
    /// </summary>
    /// <param name="departmentId">Идентификатор подразделения</param>
    /// <param name="locationId">Идентификатор локации</param>
    public DepartmentLocation(DepartmentId departmentId, LocationId locationId)
    {
        Id = new DepartmentLocationsId(Guid.NewGuid());
        DepartmentId = departmentId;
        LocationId = locationId;
    }
    
    // Ef Core
    private DepartmentLocation() { }
}