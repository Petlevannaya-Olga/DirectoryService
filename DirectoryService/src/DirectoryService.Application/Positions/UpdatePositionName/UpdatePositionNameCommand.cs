using DirectoryService.Contracts.Positions;
using Primitives.Abstractions;

namespace DirectoryService.Application.Positions.UpdatePositionName;

public sealed record UpdatePositionNameCommand(
    Guid PositionId,
    UpdatePositionNameDto Dto) : IValidation;