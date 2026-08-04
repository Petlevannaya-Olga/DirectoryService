using System.Reflection;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Http.Metadata;
using Primitives;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace DirectoryService.Presentation.EndpointResults;

public sealed class EndpointResult<TValue>
    : IResult, IEndpointMetadataProvider
{
    private readonly IResult _result;

    public EndpointResult(Result<TValue, Error> result)
    {
        _result = result.IsSuccess
            ? new SuccessResult<TValue>(result.Value)
            : new ErrorsResult<TValue>(result.Error);
    }

    public EndpointResult(Result<TValue, Errors> result)
    {
        _result = result.IsSuccess
            ? new SuccessResult<TValue>(result.Value)
            : new ErrorsResult<TValue>(result.Error);
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

        EndpointResultMetadata.AddJsonResponse(
            builder,
            StatusCodes.Status200OK,
            typeof(Envelope<TValue>));

        EndpointResultMetadata.AddErrorResponses<TValue>(builder);
    }

    public static implicit operator EndpointResult<TValue>(
        Result<TValue, Error> result)
    {
        return new EndpointResult<TValue>(result);
    }

    public static implicit operator EndpointResult<TValue>(
        Result<TValue, Errors> result)
    {
        return new EndpointResult<TValue>(result);
    }
}

public sealed class EndpointResult
    : IResult, IEndpointMetadataProvider
{
    private readonly IResult _result;

    public EndpointResult(UnitResult<Error> result)
    {
        _result = result.IsSuccess
            ? Results.NoContent()
            : new ErrorsResult<object?>(result.Error);
    }

    public EndpointResult(UnitResult<Errors> result)
    {
        _result = result.IsSuccess
            ? Results.NoContent()
            : new ErrorsResult<object?>(result.Error);
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

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

        EndpointResultMetadata.AddErrorResponses<object?>(builder);
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

internal static class EndpointResultMetadata
{
    private static readonly int[] ErrorStatusCodes =
    [
        StatusCodes.Status400BadRequest,
        StatusCodes.Status401Unauthorized,
        StatusCodes.Status403Forbidden,
        StatusCodes.Status404NotFound,
        StatusCodes.Status409Conflict,
        StatusCodes.Status500InternalServerError
    ];

    public static void AddErrorResponses<TValue>(
        EndpointBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        foreach (var statusCode in ErrorStatusCodes)
        {
            AddJsonResponse(
                builder,
                statusCode,
                typeof(Envelope<TValue>));
        }
    }

    public static void AddJsonResponse(
        EndpointBuilder builder,
        int statusCode,
        Type responseType)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(responseType);

        builder.Metadata.Add(
            new ProducesResponseTypeMetadata(
                statusCode,
                responseType,
                ["application/json"]));
    }
}