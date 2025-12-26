#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentPositions;
using Primitives;

namespace DirectoryService.Domain.Departments;

public sealed class Department
{
    private readonly List<Department> _childrenDepartments = [];
    private readonly List<DepartmentLocation> _departmentLocations = [];
    private readonly List<DepartmentPosition> _departmentPositions = [];

    /// <summary>
    /// Идентификатор, PK
    /// </summary>
    public DepartmentId Id { get; private set; }

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
    public DepartmentId? ParentId { get; private set; }

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
    public IReadOnlyList<DepartmentLocation> DepartmentLocations => _departmentLocations;

    /// <summary>
    /// Список подразделений
    /// </summary>
    public IReadOnlyList<DepartmentPosition> DepartmentPositions => _departmentPositions;

    /// <summary>
    /// Список дочерних подразделений
    /// </summary>
    public IReadOnlyList<Department> ChildrenDepartments => _childrenDepartments;

    /// <summary>
    /// Количество дочерних подразделений
    /// </summary>
    public int ChildrenCount { get; private set; }

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
    /// <param name="departmentLocations">Список локаций</param>
    private Department(
        DepartmentName name,
        Identifier identifier,
        DepartmentId? parentId,
        Path path,
        short depth,
        IEnumerable<DepartmentLocation> departmentLocations)
    {
        Id = new DepartmentId(Guid.NewGuid());
        Name = name;
        Identifier = identifier;
        ParentId = parentId;
        Path = path;
        Depth = depth;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        _departmentLocations = departmentLocations.ToList();
        ChildrenCount = _childrenDepartments.Count;
    }

    /// <summary>
    /// Создание родительского подразделения
    /// </summary>
    /// <param name="name">Название</param>
    /// <param name="identifier">Идентификатор</param>
    /// <param name="departmentLocations">Список локаций</param>
    /// <returns>Новое подразделение</returns>
    public static Result<Department, Error> CreateParent(
        DepartmentName name,
        Identifier identifier,
        IEnumerable<DepartmentLocation> departmentLocations)
    {
        var list = departmentLocations.ToList();

        if (list.Count == 0)
        {
            return CommonErrors.Validation(
                "department.location",
                "Должна быть добавлена минимум одна локация");
        }

        var path = Path.CreateParent(identifier);
        return new Department(name, identifier, null, path, 0, list);
    }

    /// <summary>
    /// Создание дочернего подразделения
    /// </summary>
    /// <param name="name">Название</param>
    /// <param name="identifier">Идентификатор</param>
    /// <param name="parent">Родительское подразделение</param>
    /// <param name="departmentLocations">Список локаций</param>
    /// <returns>Новое подразделение</returns>
    public static Result<Department, Error> CreateChild(
        DepartmentName name,
        Identifier identifier,
        Department parent,
        IEnumerable<DepartmentLocation> departmentLocations)
    {
        var list = departmentLocations.ToList();

        if (list.Count == 0)
        {
            return CommonErrors.Validation(
                "department.location",
                "Должна быть добавлена минимум одна локация");
        }

        var path = parent.Path.CreateChild(identifier);

        return new Department(
            name,
            identifier,
            parent.Id,
            path,
            (short)(parent.Depth + 1),
            list);
    }
}