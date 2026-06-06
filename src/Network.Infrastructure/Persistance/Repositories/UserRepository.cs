using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Network.Domain.Aggregates.UserAggregate;

namespace Network.Infrastructure.Persistance.Repositories;

public class UserRepository(ILogger<UserRepository> logger, NetworkDbContext dbContext) : RepositoryBase(logger, dbContext), IUserRepository
{
    #region User

    /// <summary>
    /// Add a new user to the database
    /// </summary>
    public async Task<Result<User>> AddAsync(User user)
    {
        logger.LogInformation("Creating user credentials {user}", user);

        bool userExists = await DbContext.Users.AnyAsync(item => item.EmailAddress == user.EmailAddress);

        if (userExists)
        {
            logger.LogWarning("User with email {email} already exists", user.EmailAddress);

            return Result.Fail<User>($"User with email {user.EmailAddress} already exists");
        }

        DbContext.Users.Add(user);

        await DbContext.SaveChangesAsync();

        return user;
    }

    /// <summary>
    /// Find a user by their unique identifier
    /// </summary>
    public async Task<Result<User>> FindByIdAsync(Guid id)
    {
        User? user = await DbContext.Users.FindAsync(id);

        if (user is null)
        {
            logger.LogWarning("User with id {id} was not found", id);

            return Result.Fail("User not found");
        }

        return Result.Ok(user);
    }

    /// <summary>
    /// Find a user by their email address and hashed password
    /// </summary>
    public async Task<Result<User>> FindByCredentialsAsync(string emailAddress, string hashedPassword)
    {
        User? user = await DbContext.Users
            .FirstOrDefaultAsync(item => item.EmailAddress == emailAddress
                && item.HashedPassword == hashedPassword);

        if (user is null)
        {
            logger.LogWarning("User with email {email} and password was not found", emailAddress);

            return Result.Fail("User not found");
        }

        return Result.Ok(user);
    }

    /// <summary>
    /// Remove a user from the database
    /// </summary>
    public async Task<Result> RemoveAsync(Guid id)
    {
        User? user = await DbContext.Users.FindAsync(id);

        if (user is null)
        {
            logger.LogWarning("User with id {id} was not found", id);

            return Result.Fail("User not found");
        }

        DbContext.Users.Remove(user);

        await DbContext.SaveChangesAsync();

        return Result.Ok();
    }

    /// <summary>
    /// Persist all tracked changes to the database
    /// </summary>
    public Task<Result> Save() => SaveChanges();

    #endregion

}
