namespace Primitives.Exceptions;

public class ConflictException(string message, Error[] errors) : DirectoryServiceException(message, errors)
{
}