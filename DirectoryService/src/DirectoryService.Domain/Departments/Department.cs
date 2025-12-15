#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

using CSharpFunctionalExtensions;
using Primitives;

namespace DirectoryService.Domain.Departments;

public class Department
{
    private readonly List<DepartmentLocation> _locations = [];
    private readonly List<DepartmentPosition> _positions = [];

    /// <summary>
    /// Идентификатор, PK
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Название, 3–150 симв., NOT NULL
    /// </summary>
    public DepartmentName Name { get; private set; }

    /// <summary>
    /// Идентификатор, 3–150 симв., NOT NULL, только латиница
    /// </summary>
    public Identifier Identifier { get; private set; }

    /// <summary>
    /// Головное подразделение
    /// </summary>
    public Guid? ParentId { get; private set; }

    /// <summary>
    /// Денормализованный путь (например, sales.it.dev-team)
    /// </summary>
    public Path Path { get; private set; }

    /// <summary>
    /// Глубина подразделения
    /// </summary>
    public short Depth { get; private set; }

    /// <summary>
    /// Список локаций
    /// </summary>
    public IReadOnlyList<DepartmentLocation> Locations => _locations;

    /// <summary>
    /// Список подразделений
    /// </summary>
    public IReadOnlyList<DepartmentPosition> Positions => _positions;

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
    private Department()
    {
    }

    /// <summary>
    /// Конструктор с параметрами
    /// </summary>
    /// <param name="name">Название</param>
    /// <param name="identifier">Идентификатор</param>
    /// <param name="parentId">Ссылка на родительский элемент</param>
    /// <param name="path">Денормализованный путь</param>
    /// <param name="depth">Глубина подразделения</param>
    /// <param name="locations">Список локаций</param>
    /// <param name="positions">Список позиций</param>
    private Department(
        DepartmentName name,
        Identifier identifier,
        Guid? parentId,
        Path path,
        short depth,
        IEnumerable<DepartmentLocation> locations,
        IEnumerable<DepartmentPosition> positions)
    {
        Name = name;
        Identifier = identifier;
        ParentId = parentId;
        Path = path;
        Depth = depth;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        _locations = locations.ToList();
        _positions = positions.ToList();
    }

    /// <summary>
    /// Фабричный метод
    /// </summary>
    /// <param name="name">Название</param>
    /// <param name="identifier">Идентификатор</param>
    /// <param name="parentId">Ссылка на родительский элемент</param>
    /// <param name="path">Денормализованный путь</param>
    /// <param name="depth">Глубина подразделения</param>
    /// <param name="locations">Список локаций</param>
    /// <param name="positions">Список позиций</param>
    /// <returns>Новое подразделение</returns>
    public static Result<Department, Error> Create(
        DepartmentName name,
        Identifier identifier,
        Guid? parentId,
        Path path,
        short depth,
        IEnumerable<DepartmentLocation> locations,
        IEnumerable<DepartmentPosition> positions)
    {
        return new Department(name, identifier, parentId, path, depth, locations, positions);
    }
}