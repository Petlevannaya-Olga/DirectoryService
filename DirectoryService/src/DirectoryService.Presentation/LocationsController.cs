using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations.CreateLocation;
using DirectoryService.Application.Locations.UpdateLocation;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Locations;
using DirectoryService.Presentation.EndpointResults;
using Microsoft.AspNetCore.Mvc;
using Primitives;
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

    [HttpGet("{id:guid}")]
    public async Task<EndpointResult<GetLocationDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        return Result.Success<GetLocationDto, Error>(new GetLocationDto("LocationName"));
    }

    [HttpGet]
    public async Task<EndpointResult<GetLocationDto[]>> Get(CancellationToken cancellationToken)
    {
        return Result.Success<GetLocationDto[], Error>([]);
    }

    [HttpPatch("{id:guid}")]
    public async Task<EndpointResult<Guid>> Update(
        [FromServices] ICommandHandler<Guid, UpdateLocationCommand> commandHandler,
        [FromRoute] Guid id,
        [FromBody] UpdateLocationDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLocationCommand(
            id,
            request.Name,
            request.Address,
            request.Timezone);

        return await commandHandler.Handle(
            command,
            cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult<Guid>> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        return Result.Success<Guid, Error>(id);
    }
}