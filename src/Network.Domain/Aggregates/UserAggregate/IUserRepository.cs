namespace Network.Domain.Aggregates.UserAggregate;

public interface IUserRepository
{
    #region User

    /// <summary>
    /// Add a new user to the database
    /// </summary>
    Task<Result<User>> AddAsync(User user);

    /// <summary>
    /// Find a user by their unique identifier
    /// </summary>
    Task<Result<User>> FindByIdAsync(Guid id);

    /// <summary>
    /// Find a user by their email address and hashed password
    /// </summary>
    Task<Result<User>> FindByCredentialsAsync(string emailAddress, string hashedPassword);

    /// <summary>
    /// Remove a user from the database
    /// </summary>
    Task<Result> RemoveAsync(Guid id);

    /// <summary>
    /// Persist mutations already applied to the tracked user aggregate
    /// </summary>
    Task<Result> UpdateAsync(Guid id);

    #endregion

    #region Security Note

    /// <summary>
    /// Stage a new security note to be committed as part of the current transaction
    /// </summary>
    Task AddSecurityNoteAsync(SecurityNote note);

    /// <summary>
    /// Find the security note for a given user, or null if none exists
    /// </summary>
    Task<SecurityNote?> FindSecurityNoteByUserIdAsync(Guid userId);

    /// <summary>
    /// Stage the removal of a security note to be committed as part of the current transaction
    /// </summary>
    void RemoveSecurityNote(SecurityNote note);

    #endregion
}