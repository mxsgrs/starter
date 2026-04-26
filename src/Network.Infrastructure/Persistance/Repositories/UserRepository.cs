using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Network.Domain.Aggregates.UserAggregate;

namespace Network.Infrastructure.Persistance.Repositories;

public class UserRepository(ILogger<UserRepository> logger, UserDbContext dbContext) : IUserRepository
{
    public async Task<Result<User>> CreateUser(User user)
    {
        logger.LogInformation("Creating user credentials {user}", user);

        bool userExists = await dbContext.Users.AnyAsync(item => item.EmailAddress == user.EmailAddress);

        if (userExists)
        {
            logger.LogWarning("User with email {email} already exists", user.EmailAddress);

            return Result.Fail<User>($"User with email {user.EmailAddress} already exists");
        }

        dbContext.Users.Add(user);

        await dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<Result<User>> ReadTrackedUser(Guid id)
    {
        User? user = await dbContext.Users.FindAsync(id);

        if (user is null)
        {
            logger.LogWarning("User with id {id} was not found", id);

            return Result.Fail("User not found");
        }

        return Result.Ok(user);
    }

    public async Task<Result<User>> ReadUserByCredentials(string emailAddress, string hashedPassword)
    {
        User? user = await dbContext.Users
            .FirstOrDefaultAsync(item => item.EmailAddress == emailAddress
                && item.HashedPassword == hashedPassword);

        if (user is null)
        {
            logger.LogWarning("User with email {email} and password was not found", emailAddress);

            return Result.Fail("User not found");   
        }

        return Result.Ok(user);
    }

    public async Task<Result> DeleteUser(Guid id)
    {
        User? user = await dbContext.Users.FindAsync(id);

        if (user is null)
        {
            logger.LogWarning("User with id {id} was not found", id);

            return Result.Fail("User not found");
        }

        dbContext.Users.Remove(user);

        await dbContext.SaveChangesAsync();

        return Result.Ok();
    }

    /// <summary>
    /// Persists mutations already applied to the tracked User aggregate.
    /// FindAsync returns the same in-memory instance (identity map), so no extra DB round trip occurs.
    /// </summary>
    public async Task<Result> UpdateUser(Guid id)
    {
        User? trackedUser = await dbContext.Users.FindAsync(id);

        if (trackedUser is null)
        {
            logger.LogWarning("User with id {id} was not found", id);
            return Result.Fail("User not found");
        }

        await dbContext.SaveChangesAsync();

        return Result.Ok();
    }

}
