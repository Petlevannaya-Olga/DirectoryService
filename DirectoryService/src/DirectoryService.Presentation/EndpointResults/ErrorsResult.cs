using Primitives;

namespace DirectoryService.Presentation.EndpointResults;

public sealed class ErrorsResult : IResult
{
    private readonly Errors _errors;

    public ErrorsResult(Errors errors)
    {
        _errors = errors;
    }

    public ErrorsResult(Error error)
    {
        _errors = error;
    }

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);
        if (!_errors.Any())
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return httpContext.Response.WriteAsJsonAsync(Envelope.Error(_errors));
        }

        var errorTypes = _errors
            .Select(x => x.Type)
            .Distinct()
            .ToList();

        var statusCode = errorTypes.Count > 1
            ? StatusCodes.Status500InternalServerError
            : GetStatusCodeForErrorType(errorTypes.First());

        var envelope = Envelope.Error(_errors);
        httpContext.Response.StatusCode = statusCode;

        return httpContext.Response.WriteAsJsonAsync(envelope);
    }

    private static int GetStatusCodeForErrorType(ErrorType errorType)
        =>
            errorType switch
            {
                ErrorType.VALIDATION => StatusCodes.Status400BadRequest,
                ErrorType.NOT_FOUND => StatusCodes.Status404NotFound,
                ErrorType.CONFLICT => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError,
            };
}