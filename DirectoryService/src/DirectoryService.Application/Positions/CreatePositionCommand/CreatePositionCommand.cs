using DirectoryService.Contracts;
using Primitives.Abstractions;

namespace DirectoryService.Application.Positions.CreatePositionCommand;

public record CreatePositionCommand(CreatePositionDto Dto) : IValidation;