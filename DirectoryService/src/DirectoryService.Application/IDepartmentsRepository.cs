using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;
using Primitives;

namespace DirectoryService.Application;

public interface IDepartmentsRepository
{
    Task<Result<Guid, Error>> AddAsync(Department position, CancellationToken cancellationToken);

    Task<Result<Department?, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}