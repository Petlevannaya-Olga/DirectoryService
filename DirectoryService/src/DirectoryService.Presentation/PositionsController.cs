using CSharpFunctionalExtensions;
using DirectoryService.Application.Positions.CreatePosition;
using DirectoryService.Contracts;
using DirectoryService.Presentation.EndpointResults;
using Microsoft.AspNetCore.Mvc;
using Primitives;
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

    [HttpGet("{id:guid}")]
    public async Task<EndpointResult<GetPositionDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        return Result.Success<GetPositionDto, Error>(new GetPositionDto("PositionName"));
    }

    [HttpGet]
    public async Task<EndpointResult<GetPositionDto[]>> Get(CancellationToken cancellationToken)
    {
        return Result.Success<GetPositionDto[], Error>([]);
    }

    [HttpPut("{id:guid}")]
    public async Task<EndpointResult<Guid>> Update(
        [FromRoute] Guid id,
        [FromBody] UpdatePositionDto request,
        CancellationToken cancellationToken)
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