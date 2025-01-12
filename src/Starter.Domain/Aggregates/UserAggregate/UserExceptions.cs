using Starter.Domain.Exceptions;

namespace Starter.Domain.Aggregates.UserAggregate;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(Guid id) : base($"User {id} was not found.") { }
    public UserNotFoundException(string emailAddress) : base($"User {emailAddress} was not found.") { }
}

public class InvalidUserAddressException : BadRequestException
{
    public InvalidUserAddressException(string address) : base($"User's address {address} is not valid.") { }
}
