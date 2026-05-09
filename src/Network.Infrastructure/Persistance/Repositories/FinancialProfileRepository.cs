using Microsoft.EntityFrameworkCore;
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
        FinancialProfile? profile = await dbContext.FinancialProfiles.FindAsync(id);

        if (profile is null)
        {
            logger.LogWarning("Financial profile with id {id} was not found", id);
            return Result.Fail("Financial profile not found");
        }

        await dbContext.SaveChangesAsync();
        return Result.Ok();
    }

    #endregion
}
