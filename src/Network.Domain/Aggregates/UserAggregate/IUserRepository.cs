namespace Network.Domain.Aggregates.UserAggregate;

public interface IUserRepository
{
    Task<Result<User>> CreateUser(User user);
    Task<Result<User>> ReadTrackedUser(Guid id);
    Task<Result<User>> ReadUserByCredentials(string emailAddress, string hashedPassword);
    Task<Result> DeleteUser(Guid id);
    Task<Result> UpdateUser(Guid id);
}