namespace Primitives.Exceptions;

public class InternalServerErrorException(string message, Error[] errors) : DirectoryServiceException(message, errors)
{
}