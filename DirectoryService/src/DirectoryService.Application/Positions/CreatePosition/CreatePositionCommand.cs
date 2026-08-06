using DirectoryService.Contracts;
using DirectoryService.Contracts.Positions;
using Primitives.Abstractions;

namespace DirectoryService.Application.Positions.CreatePosition;

public record CreatePositionCommand(CreatePositionDto Dto) : ICommandValidation;