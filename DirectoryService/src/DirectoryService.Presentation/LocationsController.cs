using CSharpFunctionalExtensions;
using DirectoryService.Application.Locations.Commands.CreateLocation;
using DirectoryService.Application.Locations.Commands.DeleteLocation;
using DirectoryService.Application.Locations.Commands.UpdateLocation;
using DirectoryService.Application.Locations.Queries.GetLocationById;
using DirectoryService.Application.Locations.Queries.GetLocations;
using DirectoryService.Application.Locations.Queries.GetTopLocations;
using DirectoryService.Contracts.Common;
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

    [HttpGet("{id:guid}")]
    public async Task<EndpointResult<GetLocationDto>> GetById(
        [FromServices] IQueryHandler<GetLocationDto, GetLocationByIdQuery> queryHandler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetLocationByIdQuery(id);
        return await queryHandler.Handle(query, cancellationToken);
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
            request.LocationAddress,
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

    [HttpGet("top")]
    public async Task<EndpointResult<TopLocationDto[]>> GetTop(
        [FromServices] IQueryHandler<TopLocationDto[], GetTopLocationsQuery> queryHandler,
        CancellationToken cancellationToken)
    {
        var query = new GetTopLocationsQuery();
        return await queryHandler.Handle(query, cancellationToken);
    }

    [HttpGet]
    public async Task<EndpointResult<PagedResult<LocationSummaryDto>>> GetAll(
        [FromServices] IQueryHandler<PagedResult<LocationSummaryDto>, GetLocationsQuery> queryHandler,
        [FromQuery] GetLocationsDto request,
        CancellationToken cancellationToken)
    {
        var query = new GetLocationsQuery(
            request.Search,
            request.MinDepartmentCount,
            request.SortBy,
            request.SortDir,
            request.Page,
            request.PageSize);

        return await queryHandler.Handle(query, cancellationToken);
    }
}