namespace UserService.Domain.Aggregates.UserAggregate;

public interface IUserRepository
{
    Task<Result<User>> CreateUser(User user);
    Task<Result<User>> ReadUser(Guid id);
    Task<Result<User>> ReadUser(string emailAddress, string hashedPassword);
    Task<Result<User>> UpdateUser(Guid id, User user);
}