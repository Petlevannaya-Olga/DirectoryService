using Primitives;

namespace DirectoryService.Application.Positions;

public static class PositionErrors
{
    public static Error NameConflict(string positionName)
    {
        return CommonErrors.Conflict(
            "position.name.conflict",
            $"Активная позиция с названием '{positionName}' уже существует");
    }

    public static Error DepartmentsRequired()
    {
        return CommonErrors.Validation(
            "position.departments.required",
            "Необходимо указать хотя бы одно подразделение",
            "departmentIds");
    }
}