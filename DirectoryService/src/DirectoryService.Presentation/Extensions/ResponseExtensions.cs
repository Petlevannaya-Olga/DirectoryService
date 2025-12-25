using Microsoft.AspNetCore.Mvc;
using Primitives;

namespace DirectoryService.Presentation.Extensions;

public static class ResponseExtensions
{
    public static ActionResult ToResponse(this Failure failure)
    {
        if (!failure.Any())
        {
            return new ObjectResult(null) { StatusCode = StatusCodes.Status500InternalServerError, };
        }

        var distinctErrorTypes = failure
            .Select(x => x.Type)
            .Distinct()
            .ToList();

        int statusCode = distinctErrorTypes.Count > 1
            ? StatusCodes.Status500InternalServerError
            : GetStatusCodeFromErrorType(distinctErrorTypes.First());

        return new ObjectResult(failure) { StatusCode = statusCode, };
    }

    private static int GetStatusCodeFromErrorType(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.VALIDATION => StatusCodes.Status400BadRequest,
            ErrorType.NOT_FOUND => StatusCodes.Status404NotFound,
            ErrorType.CONFLICT => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError,
        };
}