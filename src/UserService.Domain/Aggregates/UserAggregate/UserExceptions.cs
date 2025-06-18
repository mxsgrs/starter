using UserService.Domain.Exceptions;

namespace UserService.Domain.Aggregates.UserAggregate;

public class UserNotFoundException : NotFoundException
{
    public UserNotFoundException(Guid id) : base($"User {id} was not found.") { }
    public UserNotFoundException(string emailAddress) : base($"User {emailAddress} was not found.") { }
}

public class InvalidUserAddressException(string address) : BadRequestException($"User's address {address} is not valid.") { }

public class AlreadyExistingUserException(string emailAddress) : BadRequestException($"User with email address {emailAddress} already exists.") { }
