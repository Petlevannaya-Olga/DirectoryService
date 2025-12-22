using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;

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
    /// Подразделение, навигационное свойство
    /// </summary>
    public Department Department { get; private set; } = null!;

    /// <summary>
    /// Идентификатор позиции
    /// </summary>
    public Guid PositionId { get; private set; }

    /// <summary>
    /// Позиция, навигационное свойство
    /// </summary>
    public Position Position { get; private set; } = null!;

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