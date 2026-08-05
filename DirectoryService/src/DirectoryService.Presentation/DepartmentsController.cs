using DirectoryService.Application.Departments.AddLocation;
using DirectoryService.Application.Departments.AddPosition;
using DirectoryService.Application.Departments.CreateDepartment;
using DirectoryService.Application.Departments.DeleteDepartment;
using DirectoryService.Application.Departments.RemoveLocation;
using DirectoryService.Application.Departments.RemovePosition;
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

        return await commandHandler.Handle(
            command,
            cancellationToken);
    }

    [HttpGet("{departmentId:guid}")]
    public EndpointResult<GetDepartmentDto> GetById([FromRoute] Guid departmentId)
    {
        return Result.Success<GetDepartmentDto, Error>(
            new GetDepartmentDto("DepartmentName"));
    }

    [HttpGet]
    public EndpointResult<GetDepartmentDto[]> GetAll()
    {
        return Result.Success<GetDepartmentDto[], Error>([]);
    }

    [HttpPut("{departmentId:guid}/locations")]
    public async Task<EndpointResult<Guid>> UpdateLocations(
        [FromServices] ICommandHandler<Guid, UpdateLocationsCommand> commandHandler,
        [FromRoute] Guid departmentId,
        [FromBody] UpdateDepartmentLocationsDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLocationsCommand(
            departmentId,
            request.LocationIds);

        return await commandHandler.Handle(
            command,
            cancellationToken);
    }

    [HttpPatch("{departmentId:guid}")]
    public async Task<EndpointResult<Guid>> UpdateName(
        [FromServices] ICommandHandler<Guid, UpdateDepartmentCommand> commandHandler,
        [FromRoute] Guid departmentId,
        [FromBody] UpdateDepartmentNameDto request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateDepartmentCommand(
            departmentId,
            request.Name);

        return await commandHandler.Handle(
            command,
            cancellationToken);
    }

    [HttpPut("{departmentId:guid}")]
    public EndpointResult<Guid> Update(
        [FromRoute] Guid departmentId,
        [FromBody] UpdateDepartmentDto request)
    {
        return Result.Success<Guid, Error>(departmentId);
    }

    [HttpDelete("{id:guid}")]
    public async Task<EndpointResult> Delete(
        [FromServices] ICommandHandler<DeleteDepartmentCommand> commandHandler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteDepartmentCommand(id);
        return await commandHandler.Handle(command, cancellationToken);
    }

    [HttpDelete(
        "{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<EndpointResult> RemoveLocation(
        [FromServices] ICommandHandler<RemoveLocationCommand> commandHandler,
        [FromRoute] Guid departmentId,
        [FromRoute] Guid locationId,
        CancellationToken cancellationToken)
    {
        var command = new RemoveLocationCommand(
            departmentId,
            locationId);

        return await commandHandler.Handle(
            command,
            cancellationToken);
    }

    [HttpPut(
        "{departmentId:guid}/locations/{locationId:guid}")]
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
    
    [HttpPost("{departmentId:guid}/positions/{positionId:guid}")]
    public async Task<EndpointResult<Guid>> AddPosition(
        [FromServices] ICommandHandler<Guid, AddPositionCommand> commandHandler,
        [FromRoute] Guid departmentId,
        [FromRoute] Guid positionId,
        CancellationToken cancellationToken)
    {
        var command = new AddPositionCommand(departmentId, positionId);
        return await commandHandler.Handle(command, cancellationToken);
    }
    
    [HttpDelete("{departmentId:guid}/positions/{positionId:guid}")]
    public async Task<EndpointResult> RemovePosition(
        [FromServices] ICommandHandler<RemovePositionCommand> commandHandler,
        [FromRoute] Guid departmentId,
        [FromRoute] Guid positionId,
        CancellationToken cancellationToken)
    {
        var command = new RemovePositionCommand(departmentId, positionId);
        return await commandHandler.Handle(command, cancellationToken);
    }
}