using DirectoryService.Contracts.Locations;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.Commands.CreateLocation;

public record CreateLocationCommand(CreateLocationDto Dto) : ICommandValidation;