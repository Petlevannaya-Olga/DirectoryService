using Primitives.Abstractions;

namespace DirectoryService.Application.Positions.DeletePosition;

public sealed record DeletePositionCommand(Guid PositionId) : IValidation;