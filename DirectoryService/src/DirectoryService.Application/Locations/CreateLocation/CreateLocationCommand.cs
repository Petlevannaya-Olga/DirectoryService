using DirectoryService.Contracts;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.CreateLocation;

public record CreateLocationCommand(CreateLocationDto Dto) : IValidation;