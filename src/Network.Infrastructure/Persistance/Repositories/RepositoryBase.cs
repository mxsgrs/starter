using Microsoft.Extensions.Logging;

namespace Network.Infrastructure.Persistance.Repositories;

public abstract class RepositoryBase(ILogger logger, NetworkDbContext dbContext)
{
    protected NetworkDbContext DbContext { get; } = dbContext;

    /// <summary>
    /// Persist all tracked changes to the database
    /// </summary>
    protected async Task<Result> SaveChanges()
    {
        try
        {
            await DbContext.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to save changes");
            return Result.Fail("Failed to save changes");
        }
    }
}
