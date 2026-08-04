using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DirectoryService.Infrastructure.Converters;

public sealed class DepartmentIdConverter
    : ValueConverter<DepartmentId, Guid>
{
    public DepartmentIdConverter()
        : base(
            departmentId => departmentId.Value,
            value => new DepartmentId(value))
    {
    }
}