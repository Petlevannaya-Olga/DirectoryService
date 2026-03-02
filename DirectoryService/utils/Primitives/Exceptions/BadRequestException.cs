namespace Primitives.Exceptions;

public class BadRequestException(string message, Error[] errors) : DirectoryServiceException(message, errors)
{
}