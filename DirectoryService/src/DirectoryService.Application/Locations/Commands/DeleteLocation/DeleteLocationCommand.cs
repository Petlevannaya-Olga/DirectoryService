using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.Commands.DeleteLocation;

public sealed record DeleteLocationCommand(Guid LocationId) : ICommandValidation;