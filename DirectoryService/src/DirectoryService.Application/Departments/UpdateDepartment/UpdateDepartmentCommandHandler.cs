using CSharpFunctionalExtensions;
using DirectoryService.Application.Database;
using DirectoryService.Domain.Departments;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Application.Departments.UpdateDepartment;

public sealed class UpdateDepartmentCommandHandler(
    IDepartmentsRepository departmentsRepository,
    ITransactionScope unitOfWork)
    : ICommandHandler<Guid, UpdateDepartmentCommand>
{
    public async Task<Result<Guid, Errors>> Handle(UpdateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var departmentResult = await departmentsRepository.GetByIdAsync(
            command.DepartmentId,
            cancellationToken);

        if (departmentResult.IsFailure)
        {
            return departmentResult.Error.ToErrors();
        }

        if (departmentResult.Value == null)
        {
            return CommonErrors
                .NotFound("department.was.not.found", "Департамент не найден")
                .ToErrors();
        }

        var nameResult = DepartmentName.Create(command.Name);

        if (nameResult.IsFailure)
        {
            return nameResult.Error.ToErrors();
        }

        var department = departmentResult.Value;

        department.UpdateName(nameResult.Value);

        var saveResult = unitOfWork.Commit();

        if (saveResult.IsFailure)
        {
            return saveResult.Error.ToErrors();
        }

        return department.Id.Value;
    }
}