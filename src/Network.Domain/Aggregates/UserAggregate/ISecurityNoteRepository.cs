namespace Network.Domain.Aggregates.UserAggregate;

public interface ISecurityNoteRepository
{
    /// <summary>
    /// Stage a new security note to be committed as part of the current transaction
    /// </summary>
    Task AddAsync(SecurityNote note, CancellationToken cancellationToken);

    /// <summary>
    /// Find the security note for a given user, or null if none exists
    /// </summary>
    Task<SecurityNote?> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Stage the removal of a security note to be committed as part of the current transaction
    /// </summary>
    void Remove(SecurityNote note);
}
