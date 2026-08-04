using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments;
using Primitives;

namespace DirectoryService.Domain.Positions;

public sealed class Position
{
    private readonly List<DepartmentPosition> _departmentPositions = [];

    /// <summary>
    /// Идентификатор, PK
    /// </summary>
    public PositionId Id { get; private set; } = null!;

    /// <summary>
    /// Название
    /// </summary>
    public PositionName Name { get; private set; } = null!;

    /// <summary>
    /// Описание
    /// </summary>
    public Description Description { get; private set; } = null!;

    /// <summary>
    /// Список подразделений
    /// </summary>
    public IReadOnlyList<DepartmentPosition> DepartmentPositions =>
        _departmentPositions;

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

    // EF Core
    private Position()
    {
    }

    /// <summary>
    /// Конструктор с параметрами
    /// </summary>
    /// <param name="name">Название</param>
    /// <param name="description">Описание</param>
    /// <param name="departmentIds">Список идентификаторов подразделений</param>
    private Position(
        PositionName name,
        Description description,
        IEnumerable<DepartmentId> departmentIds)
    {
        var now = DateTime.UtcNow;

        Id = new PositionId(Guid.NewGuid());
        Name = name;
        Description = description;
        IsActive = true;
        CreatedAt = now;
        UpdatedAt = now;

        foreach (var departmentId in departmentIds)
        {
            _departmentPositions.Add(
                new DepartmentPosition(
                    departmentId,
                    Id));
        }
    }

    /// <summary>
    /// Создание позиции
    /// </summary>
    /// <param name="name">Название</param>
    /// <param name="description">Описание</param>
    /// <param name="departmentIds">Список идентификаторов подразделений</param>
    /// <returns>Новая позиция</returns>
    public static Result<Position, Error> Create(
        PositionName name,
        Description description,
        IEnumerable<DepartmentId> departmentIds)
    {
        var departments = departmentIds
            .Distinct()
            .ToList();

        if (departments.Count == 0)
        {
            return CommonErrors.Validation(
                "position.department",
                "Должно быть добавлено минимум одно подразделение");
        }

        return new Position(
            name,
            description,
            departments);
    }
}