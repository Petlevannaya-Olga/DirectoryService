using DirectoryService.Contracts;
using DirectoryService.Contracts.Locations;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.CreateLocation;

public record CreateLocationCommand(CreateLocationDto Dto) : IValidation;