namespace Primitives.Exceptions;

public class DirectoryServiceException(string message, Error[] errors) : Exception(message)
{
    public Error[] Errors { get; } = errors;
}