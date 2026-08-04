using CSharpFunctionalExtensions;
using DirectoryService.Application.Positions.CreatePosition;
using DirectoryService.Contracts.Positions;
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
        [FromServices]
        ICommandHandler<Guid, CreatePositionCommand> commandHandler,
        [FromBody]
        CreatePositionDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePositionCommand(request);

        return await commandHandler.Handle(
            command,
            cancellationToken);
    }

    [HttpGet("{positionId:guid}")]
    public EndpointResult<GetPositionDto> GetById(
        [FromRoute] Guid positionId)
    {
        return Result.Success<GetPositionDto, Error>(
            new GetPositionDto("PositionName"));
    }

    [HttpGet]
    public EndpointResult<GetPositionDto[]> GetAll()
    {
        return Result.Success<GetPositionDto[], Error>([]);
    }

    [HttpPut("{positionId:guid}")]
    public EndpointResult<Guid> Update(
        [FromRoute] Guid positionId,
        [FromBody] UpdatePositionDto request)
    {
        return Result.Success<Guid, Error>(positionId);
    }

    [HttpDelete("{positionId:guid}")]
    public EndpointResult<Guid> Delete(
        [FromRoute] Guid positionId)
    {
        return Result.Success<Guid, Error>(positionId);
    }
}