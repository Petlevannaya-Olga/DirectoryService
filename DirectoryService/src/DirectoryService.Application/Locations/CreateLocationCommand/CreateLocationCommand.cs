using DirectoryService.Contracts;
using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.CreateLocationCommand;

public record CreateLocationCommand(CreateLocationDto Dto) : IValidation;