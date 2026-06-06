using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Network.Domain.Aggregates.FinancialProfileAggregate;

namespace Network.Infrastructure.Persistance.Repositories;

public class FinancialProfileRepository(ILogger<FinancialProfileRepository> logger, NetworkDbContext dbContext) : RepositoryBase(logger, dbContext), IFinancialProfileRepository
{
    #region FinancialProfile

    /// <summary>
    /// Stage a new financial profile for insertion — does not call SaveChangesAsync.
    /// </summary>
    public async Task Add(FinancialProfile financialProfile)
        => await DbContext.FinancialProfiles.AddAsync(financialProfile);

    /// <summary>
    /// Return the tracked financial profile (with assets) for the given user.
    /// </summary>
    public async Task<Result<FinancialProfile>> FindByUserId(Guid userId)
    {
        FinancialProfile? profile = await DbContext.FinancialProfiles
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
    /// Persist all tracked changes to the database
    /// </summary>
    public Task<Result> Save() => SaveChanges();

    #endregion
}
