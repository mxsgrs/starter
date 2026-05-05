namespace Sales.WebApi.Persistence;

public interface IUserRepository
{
    /// <summary>
    /// Add a user to the Sales database
    /// </summary>
    Task AddAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Remove a user from the Sales database
    /// </summary>
    Task DeleteAsync(Guid userId, CancellationToken cancellationToken);
}
