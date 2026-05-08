namespace Network.Domain.Aggregates.UserAggregate;

public interface IUserRepository
{
    Task<Result<User>> AddAsync(User user);
    Task<Result<User>> FindByIdAsync(Guid id);
    Task<Result<User>> FindByCredentialsAsync(string emailAddress, string hashedPassword);
    Task<Result> RemoveAsync(Guid id);
    Task<Result> UpdateAsync(Guid id);
}