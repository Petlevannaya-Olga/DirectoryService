using CSharpFunctionalExtensions;
using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Locations;
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
    public DepartmentId Id { get; private set; } = null!;

    /// <summary>
    /// Название, 3–150 симв., NOT NULL
    /// </summary>
    public DepartmentName Name { get; private set; } = null!;

    /// <summary>
    /// Идентификатор, 3–150 симв., NOT NULL, только латиница
    /// </summary>
    public Slug Slug { get; private set; } = null!;

    /// <summary>
    /// Головное подразделение
    /// </summary>
    public DepartmentId? ParentId { get; }

    /// <summary>
    /// Денормализованный путь (например, sales.it.dev-team)
    /// </summary>
    public Path Path { get; private set; } = null!;

    /// <summary>
    /// Глубина подразделения
    /// </summary>
    public short Depth { get; }

    /// <summary>
    /// Список локаций
    /// </summary>
    public IReadOnlyList<DepartmentLocation> DepartmentLocations =>
        _departmentLocations;

    /// <summary>
    /// Список должностей
    /// </summary>
    public IReadOnlyList<DepartmentPosition> DepartmentPositions =>
        _departmentPositions;

    /// <summary>
    /// Список дочерних подразделений
    /// </summary>
    public IReadOnlyList<Department> ChildrenDepartments =>
        _childrenDepartments;

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
    /// <param name="slug">Идентификатор</param>
    /// <param name="parentId">Ссылка на родительский элемент</param>
    /// <param name="path">Денормализованный путь</param>
    /// <param name="depth">Глубина подразделения</param>
    /// <param name="locationIds">Список идентификаторов локаций</param>
    private Department(
        DepartmentName name,
        Slug slug,
        DepartmentId? parentId,
        Path path,
        short depth,
        IEnumerable<LocationId> locationIds)
    {
        var now = DateTime.UtcNow;

        Id = new DepartmentId(Guid.NewGuid());
        Name = name;
        Slug = slug;
        ParentId = parentId;
        Path = path;
        Depth = depth;
        IsActive = true;
        CreatedAt = now;
        UpdatedAt = now;
        ChildrenCount = 0;

        foreach (var locationId in locationIds)
        {
            _departmentLocations.Add(
                new DepartmentLocation(Id, locationId));
        }
    }

    /// <summary>
    /// Создание родительского подразделения
    /// </summary>
    /// <param name="name">Название</param>
    /// <param name="slug">Идентификатор</param>
    /// <param name="locationIds">Список идентификаторов локаций</param>
    /// <returns>Новое подразделение</returns>
    public static Result<Department, Error> CreateParent(
        DepartmentName name,
        Slug slug,
        IEnumerable<LocationId> locationIds)
    {
        var path = Path.CreateParent(slug);

        return Create(
            name,
            slug,
            parentId: null,
            path,
            depth: 0,
            locationIds);
    }

    /// <summary>
    /// Создание дочернего подразделения
    /// </summary>
    /// <param name="name">Название</param>
    /// <param name="slug">Идентификатор</param>
    /// <param name="parent">Родительское подразделение</param>
    /// <param name="locationIds">Список идентификаторов локаций</param>
    /// <returns>Новое подразделение</returns>
    public static Result<Department, Error> CreateChild(
        DepartmentName name,
        Slug slug,
        Department parent,
        IEnumerable<LocationId> locationIds)
    {
        var path = parent.Path.CreateChild(slug);
        var depth = checked((short)(parent.Depth + 1));

        return Create(
            name,
            slug,
            parent.Id,
            path,
            depth,
            locationIds);
    }

    /// <summary>
    /// Создание подразделения
    /// </summary>
    /// <param name="name">Название</param>
    /// <param name="slug">Идентификатор</param>
    /// <param name="parentId">Ссылка на родительский элемент</param>
    /// <param name="path">Денормализованный путь</param>
    /// <param name="depth">Глубина подразделения</param>
    /// <param name="locationIds">Список идентификаторов локаций</param>
    /// <returns>Новое подразделение</returns>
    private static Result<Department, Error> Create(
        DepartmentName name,
        Slug slug,
        DepartmentId? parentId,
        Path path,
        short depth,
        IEnumerable<LocationId> locationIds)
    {
        var locations = locationIds
            .Distinct()
            .ToList();

        if (locations.Count == 0)
        {
            return CommonErrors.Validation(
                "department.location",
                "Должна быть добавлена минимум одна локация");
        }

        return new Department(
            name,
            slug,
            parentId,
            path,
            depth,
            locations);
    }

    /// <summary>
    /// Обновляет название подразделения.
    /// Не изменяет Slug, ParentId, Path и Depth.
    /// </summary>
    /// <param name="name">Новое название подразделения.</param>
    public void UpdateName(DepartmentName name)
    {
        Name = name;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Обновляет список локаций подразделения
    /// </summary>
    /// <param name="locationIds">Новый список идентификаторов локаций</param>
    public void UpdateLocations(IEnumerable<Guid> locationIds)
    {
        var locations = locationIds
            .Distinct()
            .Select(id => new LocationId(id))
            .ToList();

        _departmentLocations.Clear();

        foreach (var locationId in locations)
        {
            _departmentLocations.Add(
                new DepartmentLocation(Id, locationId));
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Добавляет локацию в подразделение
    /// </summary>
    /// <param name="locationId">Идентификатор локации</param>
    /// <returns>Результат добавления локации</returns>
    public UnitResult<Error> AddLocation(LocationId locationId)
    {
        var relationAlreadyExists = _departmentLocations
            .Exists(relation => relation.LocationId == locationId);

        if (relationAlreadyExists)
        {
            return Errors.DepartmentLocationAlreadyExists(
                Id.Value,
                locationId.Value);
        }

        _departmentLocations.Add(
            new DepartmentLocation(Id, locationId));

        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<Error>();
    }

    /// <summary>
    /// Удаляет локацию из подразделения
    /// </summary>
    /// <param name="locationId">Идентификатор локации</param>
    /// <returns>Результат удаления локации</returns>
    public UnitResult<Error> RemoveLocation(LocationId locationId)
    {
        var departmentLocation = _departmentLocations
            .FirstOrDefault(relation =>
                relation.LocationId == locationId);

        if (departmentLocation is null)
        {
            return Errors.LocationNotAttached(
                Id.Value,
                locationId.Value);
        }

        _departmentLocations.Remove(departmentLocation);

        UpdatedAt = DateTime.UtcNow;

        return UnitResult.Success<Error>();
    }

    private static class Errors
    {
        public static Error DepartmentLocationAlreadyExists(
            Guid departmentId,
            Guid locationId)
        {
            return new Error(
                "department.location.already.exists",
                $"Локация '{locationId}' уже привязана к подразделению '{departmentId}'",
                ErrorType.CONFLICT,
                "locationId");
        }

        public static Error LocationNotAttached(
            Guid departmentId,
            Guid locationId)
        {
            return new Error(
                "department.location.not.attached",
                $"Локация '{locationId}' не привязана к подразделению '{departmentId}'",
                ErrorType.NOTFOUND,
                "locationId");
        }
    }
}