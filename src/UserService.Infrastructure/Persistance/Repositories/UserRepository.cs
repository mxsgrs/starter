using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Domain.Aggregates.UserAggregate;

namespace UserService.Infrastructure.Persistance.Repositories;

public class UserRepository(ILogger<UserRepository> logger, UserDbContext dbContext) : IUserRepository
{
    private readonly ILogger<UserRepository> _logger = logger;
    private readonly UserDbContext _dbContext = dbContext;

    public async Task<Result<User>> CreateUser(User user)
    {
        _logger.LogInformation("Creating user credentials {user}", user);

        bool userExists = await _dbContext.Users.AnyAsync(item => item.EmailAddress == user.EmailAddress);

        if (userExists)
        {
            _logger.LogWarning("User with email {email} already exists", user.EmailAddress);

            return Result.Fail<User>($"User with email {user.EmailAddress} already exists");
        }

        _dbContext.Users.Add(user);

        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<Result<User>> ReadUser(Guid id)
    {
        User? user = await _dbContext.Users.FindAsync(id);

        if (user is null)
        {
            _logger.LogWarning("User with id {id} was not found", id);

            return Result.Fail("User not found");
        }

        return Result.Ok(user);
    }

    public async Task<Result<User>> ReadUser(string emailAddress, string hashedPassword)
    {
        User? user = await _dbContext.Users
            .FirstOrDefaultAsync(item => item.EmailAddress == emailAddress
                && item.HashedPassword == hashedPassword);

        if (user is null)
        {
            _logger.LogWarning("User with email {email} and password was not found", emailAddress);

            return Result.Fail("User not found");   
        }

        return Result.Ok(user);
    }

    public async Task<Result<User>> UpdateUser(Guid id, User user)
    {
        User? existing = await _dbContext.Users
            .FirstOrDefaultAsync(item => item.EmailAddress == user.EmailAddress
                && item.HashedPassword == user.HashedPassword);

        if (existing is null)
        {
            _logger.LogWarning("User with id {id} was not found", id);

            return Result.Fail("User not found");
        }

        _dbContext.Entry(existing).CurrentValues.SetValues(user);

        // Update the user address as owned types are not updated by default
        if (user.Address is not null && existing.Address is not null)
        {
            _dbContext.Entry(existing.Address).CurrentValues.SetValues(user.Address);
        }

        await _dbContext.SaveChangesAsync();

        return Result.Ok(existing);
    }
}
