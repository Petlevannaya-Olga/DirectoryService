using DirectoryService.Application.Locations.CreateLocationCommand;
using DirectoryService.Contracts;
using DirectoryService.Presentation.EndpointResults;
using DirectoryService.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;
using Primitives.Abstractions;

namespace DirectoryService.Presentation;

[ApiController]
[Route("api/locations")]
public sealed class LocationsController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ICommandHandler<Guid, CreateLocationCommand> commandHandler,
        [FromBody] CreateLocationDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLocationCommand(request);
        return await commandHandler.Handle(command, cancellationToken);
    }
}