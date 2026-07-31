using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.UpdateLocations;

public sealed record UpdateLocationsCommand(Guid DepartmentId, IEnumerable<Guid> LocationIds) : IValidation;