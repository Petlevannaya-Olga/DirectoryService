using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.DeleteLocation;

public sealed record DeleteLocationCommand(Guid LocationId) : IValidation;