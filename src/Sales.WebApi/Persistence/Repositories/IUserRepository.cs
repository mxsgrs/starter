namespace Sales.WebApi.Persistence.Repositories;

public interface IUserRepository
{
    /// <summary>
    /// Add a user to the Sales database
    /// </summary>
    Task Add(Guid userId);

    /// <summary>
    /// Remove a user from the Sales database
    /// </summary>
    Task Delete(Guid userId);
}
