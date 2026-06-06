using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Network.Domain.Aggregates.FinancialProfileAggregate;

namespace Network.Infrastructure.Persistance.Repositories;

public class FinancialProfileRepository(ILogger<FinancialProfileRepository> logger, NetworkDbContext dbContext) : IFinancialProfileRepository
{
    #region FinancialProfile

    /// <summary>
    /// Stage a new financial profile for insertion — does not call SaveChangesAsync.
    /// </summary>
    public async Task AddAsync(FinancialProfile financialProfile)
        => await dbContext.FinancialProfiles.AddAsync(financialProfile);

    /// <summary>
    /// Return the tracked financial profile (with assets) for the given user.
    /// </summary>
    public async Task<Result<FinancialProfile>> FindByUserIdAsync(Guid userId)
    {
        FinancialProfile? profile = await dbContext.FinancialProfiles
            .Include(fp => fp.Assets)
            .FirstOrDefaultAsync(fp => fp.UserId == userId);

        if (profile is null)
        {
            logger.LogWarning("Financial profile for user {userId} was not found", userId);
            return Result.Fail("Financial profile not found");
        }

        return Result.Ok(profile);
    }

    /// <summary>
    /// Persist tracked changes for the financial profile with the given id.
    /// </summary>
    public async Task<Result> UpdateAsync(Guid id)
    {
        EntityEntry<FinancialProfile>? entry = dbContext.ChangeTracker
            .Entries<FinancialProfile>()
            .FirstOrDefault(e => e.Entity.Id == id);

        if (entry is null || entry.State == EntityState.Unchanged)
        {
            logger.LogWarning("Financial profile with id {id} was not tracked with modifications", id);
            return Result.Fail("Financial profile not found");
        }

        await dbContext.SaveChangesAsync();
        return Result.Ok();
    }

    #endregion
}
