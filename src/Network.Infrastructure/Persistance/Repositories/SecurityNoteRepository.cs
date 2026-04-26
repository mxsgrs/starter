using Microsoft.EntityFrameworkCore;
using Network.Domain.Aggregates.UserAggregate;

namespace Network.Infrastructure.Persistance.Repositories;

public class SecurityNoteRepository(UserDbContext dbContext) : ISecurityNoteRepository
{
    /// <summary>
    /// Stage a new security note to be committed as part of the current transaction
    /// </summary>
    public async Task AddAsync(SecurityNote note, CancellationToken cancellationToken)
        => await dbContext.SecurityNotes.AddAsync(note, cancellationToken);

    /// <summary>
    /// Find the security note for a given user, or null if none exists
    /// </summary>
    public async Task<SecurityNote?> FindByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        => await dbContext.SecurityNotes.FirstOrDefaultAsync(n => n.UserId == userId, cancellationToken);

    /// <summary>
    /// Stage the removal of a security note to be committed as part of the current transaction
    /// </summary>
    public void Remove(SecurityNote note)
        => dbContext.SecurityNotes.Remove(note);
}
