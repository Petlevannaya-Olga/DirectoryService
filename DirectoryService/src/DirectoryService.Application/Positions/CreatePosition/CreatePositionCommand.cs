using DirectoryService.Contracts;
using Primitives.Abstractions;

namespace DirectoryService.Application.Positions.CreatePosition;

public record CreatePositionCommand(CreatePositionDto Dto) : IValidation;