using DirectoryService.Domain.DepartmentPositions;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace DirectoryService.Domain.Positions;

public sealed class Position
{
    private readonly List<DepartmentPosition> _departmentPositions;

    /// <summary>
    /// Идентификатор, PK
    /// </summary>
    public PositionId Id { get; private set; }

    /// <summary>
    /// Название
    /// </summary>
    public PositionName Name { get; private set; }

    /// <summary>
    /// Описание
    /// </summary>
    public Description Description { get; private set; }

    /// <summary>
    /// Список подразделений
    /// </summary>
    public IReadOnlyList<DepartmentPosition> DepartmentPositions => _departmentPositions;

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
    /// <param name="description">Описание</param>
    /// <param name="departments">Список подразделений</param>
    public Position(
        PositionName name,
        Description description,
        IEnumerable<DepartmentPosition> departments)
    {
        Id = new PositionId(Guid.NewGuid());
        Name = name;
        Description = description;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        _departmentPositions = departments.ToList();
    }

    // EF Core
    private Position() { }
}