using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments.CreateDepartment;
using DirectoryService.Application.Locations.UpdateLocations;
using DirectoryService.Contracts;
using DirectoryService.Presentation.EndpointResults;
using Microsoft.AspNetCore.Mvc;
using Primitives;
using Primitives.Abstractions;

namespace DirectoryService.Presentation;

[ApiController]
[Route("api/departments")]
public sealed class DepartmentsController : ControllerBase
{
    [HttpPost]
    public async Task<EndpointResult<Guid>> Create(
        [FromServices] ICommandHandler<Guid, CreateDepartmentCommand> commandHandler,
        [FromBody] CreateDepartmentDto request,
        CancellationToken cancellationToken)
    {
        var command = new CreateDepartmentCommand(request);
        return await commandHandler.Handle(command, cancellationToken);
    }

    [HttpGet("{id:guid}")]
    public async Task<EndpointResult<GetDepartmentDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        return Result.Success<GetDepartmentDto, Error>(new GetDepartmentDto("DepartmentName"));
    }

    [HttpGet]
    public async Task<EndpointResult<GetDepartmentDto[]>> Get(CancellationToken cancellationToken)
    {
        return Result.Success<GetDepartmentDto[], Error>([]);
    }

    [HttpPatch("{id:guid}/locations")]
    public async Task<EndpointResult<Guid>> UpdateLocations(
        [FromServices] ICommandHandler<Guid, UpdateLocationsCommand> commandHandler,
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentLocationsDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLocationsCommand(id, request.LocationIds);
        return await commandHandler.Handle(command, cancellationToken);
    }

    [HttpPut("{id:guid}")]
    public async Task<EndpointResult<Guid>> Update([FromRoute] Guid id, [FromBody] UpdateDepartmentDto request)
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