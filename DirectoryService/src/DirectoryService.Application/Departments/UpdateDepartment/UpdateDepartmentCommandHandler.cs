using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Departments;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.UpdateDepartment;

public sealed class UpdateDepartmentCommandHandler(
    IDepartmentsRepository departmentsRepository,
    ITransactionManager transactionManager)
    : ICommandHandler<Guid, UpdateDepartmentCommand>
{
    public async Task<Result<Guid, Errors>> Handle(
        UpdateDepartmentCommand command,
        CancellationToken cancellationToken)
    {
        var departmentResult =
            await departmentsRepository.GetByIdAsync(
                command.DepartmentId,
                cancellationToken);

        if (departmentResult.IsFailure)
        {
            return departmentResult.Error.ToErrors();
        }

        var department = departmentResult.Value;
        var name = DepartmentName.Create(command.Name).Value;

        if (department.Name == name)
        {
            return department.Id.Value;
        }

        department.UpdateName(name);

        var saveResult =
            await transactionManager.SaveChangesAsync(
                cancellationToken);

        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        return department.Id.Value;
    }
}