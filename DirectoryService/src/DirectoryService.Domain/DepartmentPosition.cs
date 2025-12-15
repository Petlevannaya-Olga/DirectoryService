namespace DirectoryService.Domain;

/// <summary>
/// Связь многие-ко-многим между подразделениями и позициями
/// </summary>
public class DepartmentPosition
{
    /// <summary>
    /// Инентификатор подразделения
    /// </summary>
    public Guid DepartmentId { get; private set; }

    /// <summary>
    /// Идентификатор позиции
    /// </summary>
    public Guid PositionId { get; private set; }

    /// <summary>
    /// Конструктор с параметрами
    /// </summary>
    /// <param name="departmentId">Идентификатор подразделения</param>
    /// <param name="positionId">Идентификатор позиции</param>
    public DepartmentPosition(Guid departmentId, Guid positionId)
    {
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    // EF Core
    private DepartmentPosition() { }
}