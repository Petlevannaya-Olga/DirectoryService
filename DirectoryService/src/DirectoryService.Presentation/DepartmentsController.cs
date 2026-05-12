using DirectoryService.Application.Departments.CreateDepartmentCommand;
using DirectoryService.Application.Locations.UpdateLocationsCommand;
using DirectoryService.Contracts;
using DirectoryService.Presentation.EndpointResults;
using Microsoft.AspNetCore.Mvc;
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

    [HttpPut("{departmentId:guid}/locations")]
    public async Task<EndpointResult<Guid>> Update(
        [FromServices] ICommandHandler<Guid, UpdateLocationsCommand> commandHandler,
        [FromRoute] Guid departmentId,
        [FromBody] IEnumerable<Guid> locations,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLocationsCommand(departmentId, locations);
        return await commandHandler.Handle(command, cancellationToken);
    }
}