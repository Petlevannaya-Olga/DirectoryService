using Primitives.Abstractions;

namespace DirectoryService.Application.Locations.Queries.GetLocationById;

public sealed record GetLocationByIdQuery(Guid LocationId) : IQuery;