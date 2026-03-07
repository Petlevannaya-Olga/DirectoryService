using DirectoryService.Application.Positions.CreatePositionCommand;
using DirectoryService.Contracts;
using DirectoryService.Presentation.EndpointResults;
using Microsoft.AspNetCore.Mvc;
using Primitives.Abstractions;

namespace DirectoryService.Presentation;

[ApiController]
[Route("api/positions")]
public sealed class PositionsController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ICommandHandler<Guid, CreatePositionCommand> commandHandler,
        [FromBody] CreatePositionDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePositionCommand(request);
        return await commandHandler.Handle(command, cancellationToken);
    }
}