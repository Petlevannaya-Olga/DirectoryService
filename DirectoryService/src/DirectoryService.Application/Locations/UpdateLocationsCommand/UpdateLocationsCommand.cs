using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.UpdateLocationsCommand;

public sealed record UpdateLocationsCommand(Guid DepartmentId, IEnumerable<Guid> LocationIds) : IValidation;