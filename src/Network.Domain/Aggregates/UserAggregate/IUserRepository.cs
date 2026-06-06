namespace Network.Domain.Aggregates.UserAggregate;

public interface IUserRepository
{
    #region User

    /// <summary>
    /// Add a new user to the database
    /// </summary>
    Task<Result<User>> Add(User user);

    /// <summary>
    /// Find a user by their unique identifier
    /// </summary>
    Task<Result<User>> FindById(Guid id);

    /// <summary>
    /// Find a user by their email address and hashed password
    /// </summary>
    Task<Result<User>> FindByCredentials(string emailAddress, string hashedPassword);

    /// <summary>
    /// Remove a user from the database
    /// </summary>
    Task<Result> Remove(Guid id);

    /// <summary>
    /// Persist all tracked changes to the database
    /// </summary>
    Task<Result> Save();

    #endregion

}