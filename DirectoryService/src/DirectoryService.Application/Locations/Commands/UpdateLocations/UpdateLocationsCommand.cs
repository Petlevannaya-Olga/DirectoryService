using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.Commands.UpdateLocations;

public sealed record UpdateLocationsCommand(Guid DepartmentId, IEnumerable<Guid> LocationIds) : ICommandValidation;