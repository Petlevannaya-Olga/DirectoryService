using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Departments;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.UpdateDepartment;

public sealed class UpdateDepartmentHandler(
    IDepartmentsRepository departmentsRepository,
    ITransactionManager transactionManager,
    ILogger<UpdateDepartmentHandler> logger)
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

        var nameResult =
            DepartmentName.Create(command.Name);

        if (nameResult.IsFailure)
        {
            return nameResult.Error.ToErrors();
        }

        var department = departmentResult.Value;
        var name = nameResult.Value;

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

        logger.LogInformation(
            "Данные подразделения {DepartmentId} были обновлены",
            department.Id.Value);

        return department.Id.Value;
    }
}