namespace Primitives.Exceptions;

public class NotFoundException(string message, Guid id, Error[] errors) : DirectoryServiceException(message, errors)
{
    public Guid Id { get; } = id;
}