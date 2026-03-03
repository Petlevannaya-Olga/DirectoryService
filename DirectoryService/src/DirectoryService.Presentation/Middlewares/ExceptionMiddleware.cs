using Primitives;
using Primitives.Exceptions;

namespace DirectoryService.Presentation.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(context, e);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        logger.LogError(exception, exception.Message);

        var directoryServiceException = exception as DirectoryServiceException;

        (int code, Error[]? errors) = directoryServiceException switch
        {
            InternalServerErrorException => (
                StatusCodes.Status500InternalServerError,
                directoryServiceException.Errors),

            NotFoundException => (
                StatusCodes.Status404NotFound,
                directoryServiceException.Errors),

            BadRequestException => (
                StatusCodes.Status404NotFound,
                directoryServiceException.Errors),

            ConflictException => (
                StatusCodes.Status409Conflict,
                directoryServiceException.Errors),

            _ => (
                StatusCodes.Status500InternalServerError,
                [CommonErrors.Failure("internal.server.error", "Internal server error")]),
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = code;

        await context.Response.WriteAsJsonAsync(errors);
    }
}