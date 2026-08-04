using Primitives;

namespace DirectoryService.Presentation.EndpointResults;

public sealed class ErrorsResult<TValue> : IResult
{
    private readonly Errors _errors;

    public ErrorsResult(Errors errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        _errors = errors;
    }

    public ErrorsResult(Error error)
        : this(error.ToErrors())
    {
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var statusCode = GetStatusCode(_errors);
        var envelope = Envelope<TValue>.Error(_errors);

        httpContext.Response.StatusCode = statusCode;

        return httpContext.Response.WriteAsJsonAsync(
            envelope,
            httpContext.RequestAborted);
    }

    private static int GetStatusCode(Errors errors)
    {
        if (errors.Count == 0)
        {
            return StatusCodes.Status500InternalServerError;
        }

        var errorTypes = errors
            .Select(error => error.Type)
            .Distinct()
            .ToList();

        if (errorTypes.Count != 1)
        {
            return StatusCodes.Status500InternalServerError;
        }

        return GetStatusCodeForErrorType(errorTypes[0]);
    }

    private static int GetStatusCodeForErrorType(
        ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.VALIDATION =>
                StatusCodes.Status400BadRequest,

            ErrorType.NOTFOUND =>
                StatusCodes.Status404NotFound,

            ErrorType.CONFLICT =>
                StatusCodes.Status409Conflict,

            _ =>
                StatusCodes.Status500InternalServerError
        };
    }
}