using DirectoryService.Application.Departments.AddLocation;
using DirectoryService.Application.Departments.CreateDepartment;
using DirectoryService.Application.Departments.RemoveLocation;
using DirectoryService.Application.Departments.UpdateDepartment;
using DirectoryService.Application.Locations.UpdateLocations;
using DirectoryService.Contracts.Departments;
using DirectoryService.Presentation.EndpointResults;
using Microsoft.AspNetCore.Mvc;
using Primitives;
using Primitives.Abstractions;
using Result = CSharpFunctionalExtensions.Result;

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

    [HttpPut("{id:guid}/locations")]
    public async Task<EndpointResult<Guid>> UpdateLocations(
        [FromServices] ICommandHandler<Guid, UpdateLocationsCommand> commandHandler,
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentLocationsDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLocationsCommand(id, request.LocationIds);
        return await commandHandler.Handle(command, cancellationToken);
    }

    [HttpPatch("{id:guid}")]
    public async Task<EndpointResult<Guid>> Update(
        [FromServices] ICommandHandler<Guid, UpdateDepartmentCommand> commandHandler,
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentNameDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDepartmentCommand(id, request.Name);
        return await commandHandler.Handle(command, cancellationToken);
    }


    [HttpPut("{id:guid}")]
    public async Task<EndpointResult<Guid>> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentDto request)
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
    
    [HttpDelete("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<EndpointResult> RemoveLocation(
        [FromServices]
        ICommandHandler<RemoveLocationCommand> commandHandler,
        [FromRoute]
        Guid departmentId,
        [FromRoute]
        Guid locationId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveLocationCommand(
            departmentId,
            locationId);

        return await commandHandler.Handle(
            command,
            cancellationToken);
    }

    [HttpPost("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<EndpointResult<Guid>> AddLocation(
        [FromServices] ICommandHandler<Guid, AddLocationCommand> commandHandler,
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        var command = new AddLocationCommand(
            departmentId,
            locationId);

        return await commandHandler.Handle(
            command,
            cancellationToken);
    }
}