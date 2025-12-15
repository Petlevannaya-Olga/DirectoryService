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
    /// Идентификатор локации
    /// </summary>
    public Guid LocationId { get; private set; }

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