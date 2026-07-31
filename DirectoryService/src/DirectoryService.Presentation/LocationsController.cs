using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations.CreateLocation;
using DirectoryService.Contracts;
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

    [HttpPut("{id:guid}")]
    public async Task<EndpointResult<Guid>> Update([FromRoute] Guid id, [FromBody] UpdateLocationDto request)
    {
        return Result.Success<Guid, Error>(id);
    }

    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult<Guid>> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        return Result.Success<Guid, Error>(id);
    }
}