using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using Microsoft.Extensions.Logging;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.DeleteDepartment;

public sealed class DeleteDepartmentHandler(
    IDepartmentsRepository departmentsRepository,
    ITransactionManager transactionManager,
    ILogger<DeleteDepartmentHandler> logger)
    : ICommandHandler<DeleteDepartmentCommand>
{
    public async Task<UnitResult<Errors>> Handle(
        DeleteDepartmentCommand command,
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

        if (!department.IsActive)
        {
            return UnitResult.Success<Errors>();
        }

        department.Deactivate();

        var saveResult =
            await transactionManager.SaveChangesAsync(
                cancellationToken);

        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        logger.LogInformation(
            "Подразделение {DepartmentId} удалено",
            department.Id.Value);

        return UnitResult.Success<Errors>();
    }
}