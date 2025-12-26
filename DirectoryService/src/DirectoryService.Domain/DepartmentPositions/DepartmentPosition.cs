using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;

namespace DirectoryService.Domain.DepartmentPositions;

/// <summary>
/// Связь многие-ко-многим между подразделениями и позициями
/// </summary>
public sealed class DepartmentPosition
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public DepartmentPositionsId Id { get; private set; }

    /// <summary>
    /// Инентификатор подразделения
    /// </summary>
    public DepartmentId DepartmentId { get; private set; }

    /// <summary>
    /// Идентификатор позиции
    /// </summary>
    public PositionId PositionId { get; private set; }

    /// <summary>
    /// Конструктор с параметрами
    /// </summary>
    /// <param name="departmentId">Идентификатор подразделения</param>
    /// <param name="positionId">Идентификатор позиции</param>
    public DepartmentPosition(DepartmentId departmentId, PositionId positionId)
    {
        Id = new DepartmentPositionsId(Guid.NewGuid());
        DepartmentId = departmentId;
        PositionId = positionId;
    }

    // Ef Core
    private DepartmentPosition() { }
}