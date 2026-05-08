using Microsoft.EntityFrameworkCore;
using Sales.WebApi.Domain;

namespace Sales.WebApi.Persistence.Repositories;

public class UserRepository(SalesDbContext dbContext) : IUserRepository
{
    /// <summary>
    /// Add a user to the Sales database
    /// </summary>
    public async Task AddAsync(Guid userId)
    {
        dbContext.Users.Add(User.Register(userId));
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Remove a user from the Sales database
    /// </summary>
    public async Task DeleteAsync(Guid userId)
        => await dbContext.Users
            .Where(u => u.Id == userId)
            .ExecuteDeleteAsync();
}
