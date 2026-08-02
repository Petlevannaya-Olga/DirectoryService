using System.Reflection;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http.Metadata;
using Primitives;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace DirectoryService.Presentation.EndpointResults;

public sealed class EndpointResult<TValue> : IResult, IEndpointMetadataProvider
{
    private readonly IResult _result;

    public EndpointResult(Result<TValue, Error> result)
    {
        _result = result.IsSuccess
            ? new SuccessResult<TValue>(result.Value)
            : new ErrorsResult(result.Error);
    }

    public EndpointResult(Result<TValue, Errors> result)
    {
        _result = result.IsSuccess
            ? new SuccessResult<TValue>(result.Value)
            : new ErrorsResult(result.Error);
    }

    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(builder);

        builder.Metadata.Add(new ProducesResponseTypeMetadata(200, typeof(Envelope<TValue>), ["application/json"]));
        builder.Metadata.Add(new ProducesResponseTypeMetadata(400, typeof(Envelope<TValue>), ["application/json"]));
        builder.Metadata.Add(new ProducesResponseTypeMetadata(401, typeof(Envelope<TValue>), ["application/json"]));
        builder.Metadata.Add(new ProducesResponseTypeMetadata(403, typeof(Envelope<TValue>), ["application/json"]));
        builder.Metadata.Add(new ProducesResponseTypeMetadata(404, typeof(Envelope<TValue>), ["application/json"]));
        builder.Metadata.Add(new ProducesResponseTypeMetadata(409, typeof(Envelope<TValue>), ["application/json"]));
        builder.Metadata.Add(new ProducesResponseTypeMetadata(500, typeof(Envelope<TValue>), ["application/json"]));
    }

    public Task ExecuteAsync(HttpContext httpContext) => _result.ExecuteAsync(httpContext);

    public static implicit operator EndpointResult<TValue>(Result<TValue, Error> result)
        => new(result);

    public static implicit operator EndpointResult<TValue>(Result<TValue, Errors> result)
        => new(result);
}

public sealed class EndpointResult
    : IResult, IEndpointMetadataProvider
{
    private readonly IResult _result;

    public EndpointResult(UnitResult<Error> result)
    {
        _result = result.IsSuccess
            ? Results.NoContent()
            : new ErrorsResult(result.Error);
    }

    public EndpointResult(UnitResult<Errors> result)
    {
        _result = result.IsSuccess
            ? Results.NoContent()
            : new ErrorsResult(result.Error);
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        return _result.ExecuteAsync(httpContext);
    }

    public static void PopulateMetadata(
        MethodInfo method,
        EndpointBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(builder);

        builder.Metadata.Add(
            new ProducesResponseTypeMetadata(
                StatusCodes.Status204NoContent));

        var errorType = typeof(Envelope<Errors>);

        builder.Metadata.Add(
            new ProducesResponseTypeMetadata(
                StatusCodes.Status400BadRequest,
                errorType,
                ["application/json"]));

        builder.Metadata.Add(
            new ProducesResponseTypeMetadata(
                StatusCodes.Status401Unauthorized,
                errorType,
                ["application/json"]));

        builder.Metadata.Add(
            new ProducesResponseTypeMetadata(
                StatusCodes.Status403Forbidden,
                errorType,
                ["application/json"]));

        builder.Metadata.Add(
            new ProducesResponseTypeMetadata(
                StatusCodes.Status404NotFound,
                errorType,
                ["application/json"]));

        builder.Metadata.Add(
            new ProducesResponseTypeMetadata(
                StatusCodes.Status409Conflict,
                errorType,
                ["application/json"]));

        builder.Metadata.Add(
            new ProducesResponseTypeMetadata(
                StatusCodes.Status500InternalServerError,
                errorType,
                ["application/json"]));
    }

    public static implicit operator EndpointResult(
        UnitResult<Error> result)
    {
        return new EndpointResult(result);
    }

    public static implicit operator EndpointResult(
        UnitResult<Errors> result)
    {
        return new EndpointResult(result);
    }
}