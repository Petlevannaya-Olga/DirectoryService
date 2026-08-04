using Primitives;

namespace DirectoryService.Application.Departments;

public static partial class DepartmentErrors
{
    public static Error NotFound(Guid id)
    {
        return CommonErrors.NotFound(
            "department.not.found",
            $"Подразделение с id = {id} не найдено",
            id);
    }
}