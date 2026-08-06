using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Departments;
using Microsoft.EntityFrameworkCore;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.Queries.GetDepartmentById;

public sealed class GetDepartmentByIdHandler(
    IReadDbContext readDbContext)
    : IQueryHandler<
        Result<GetDepartmentDto, Errors>,
        GetDepartmentByIdQuery>
{
    public async Task<Result<GetDepartmentDto, Errors>> Handle(
        GetDepartmentByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var departmentId =
            new DepartmentId(query.DepartmentId);

        var department = await readDbContext
            .DepartmentsRead
            .Where(item =>
                item.Id == departmentId &&
                item.IsActive)
            .Select(item => new GetDepartmentDto(
                item.Id.Value,
                item.Name.Value,
                item.Slug.Value,
                item.IsActive,
                item.CreatedAt,
                item.UpdatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        if (department is null)
        {
            return CommonErrors
                .NotFound(
                    "department.not.found",
                    $"Подразделение с идентификатором '{query.DepartmentId}' не найдено",
                    query.DepartmentId)
                .ToErrors();
        }

        return department;
    }
}