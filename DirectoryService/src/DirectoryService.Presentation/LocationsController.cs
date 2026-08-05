using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations.CreateLocation;
using DirectoryService.Application.Locations.DeleteLocation;
using DirectoryService.Application.Locations.UpdateLocation;
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

        return await commandHandler.Handle(
            command,
            cancellationToken);
    }

    [HttpGet("{locationId:guid}")]
    public EndpointResult<GetLocationDto> GetById([FromRoute] Guid locationId)
    {
        return Result.Success<GetLocationDto, Error>(
            new GetLocationDto("LocationName"));
    }

    [HttpGet]
    public EndpointResult<GetLocationDto[]> GetAll()
    {
        return Result.Success<GetLocationDto[], Error>([]);
    }

    [HttpPut("{locationId:guid}")]
    public async Task<EndpointResult<Guid>> Update(
        [FromServices] ICommandHandler<Guid, UpdateLocationCommand> commandHandler,
        [FromRoute] Guid locationId,
        [FromBody] UpdateLocationDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLocationCommand(
            locationId,
            request.Name,
            request.Address,
            request.Timezone);

        return await commandHandler.Handle(
            command,
            cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult> Delete(
        [FromServices] ICommandHandler<DeleteLocationCommand> commandHandler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteLocationCommand(id);
        return await commandHandler.Handle(command, cancellationToken);
    }
}