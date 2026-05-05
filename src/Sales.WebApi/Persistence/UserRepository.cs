using Microsoft.EntityFrameworkCore;

namespace Sales.WebApi.Persistence;

public class UserRepository(SalesDbContext dbContext) : IUserRepository
{
    /// <summary>
    /// Add a user to the Sales database
    /// </summary>
    public async Task AddAsync(Guid userId, CancellationToken cancellationToken)
    {
        dbContext.Users.Add(User.Register(userId));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Remove a user from the Sales database
    /// </summary>
    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken)
        => await dbContext.Users
            .Where(u => u.Id == userId)
            .ExecuteDeleteAsync(cancellationToken);
}
