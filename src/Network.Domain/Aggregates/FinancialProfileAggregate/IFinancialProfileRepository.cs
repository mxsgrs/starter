namespace Network.Domain.Aggregates.FinancialProfileAggregate;

public interface IFinancialProfileRepository
{
    #region FinancialProfile

    /// <summary>
    /// Stage a new financial profile for insertion — does not call SaveChangesAsync.
    /// </summary>
    Task AddAsync(FinancialProfile financialProfile);

    /// <summary>
    /// Return the tracked financial profile for the given user.
    /// </summary>
    Task<Result<FinancialProfile>> FindByUserIdAsync(Guid userId);

    /// <summary>
    /// Persist all tracked changes to the database
    /// </summary>
    Task<Result> Save();

    #endregion
}
