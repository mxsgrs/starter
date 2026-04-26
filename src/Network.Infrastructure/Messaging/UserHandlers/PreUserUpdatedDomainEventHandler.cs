using Microsoft.EntityFrameworkCore;
using Network.Application.Shared.Events;
using Network.Domain.Aggregates.UserAggregate;
using Network.Infrastructure.Persistance;

namespace Network.Infrastructure.Messaging.UserHandlers;

public class PreUserUpdatedDomainEventHandler(UserDbContext dbContext)
    : IPreSavedDomainEventHandler<UserUpdatedDomainEvent>
{
    /// <summary>
    /// Write an audit log entry when a user is updated, and create or delete their security note based on age — committed atomically
    /// </summary>
    public async Task HandleAsync(UserUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        UserAuditLog auditLog = UserAuditLog.Create(domainEvent.UserId, nameof(UserUpdatedDomainEvent));
        await dbContext.AuditLogs.AddAsync(auditLog, cancellationToken);

        User? user = dbContext.Users.Local.FirstOrDefault(u => u.Id == domainEvent.UserId);
        if (user is null) return;

        SecurityNote? existingNote = await dbContext.SecurityNotes
            .FirstOrDefaultAsync(n => n.UserId == domainEvent.UserId, cancellationToken);

        if (user.Age >= 30 && existingNote is null)
        {
            SecurityNote note = SecurityNote.Create(domainEvent.UserId, "User age is 30 or above");
            await dbContext.SecurityNotes.AddAsync(note, cancellationToken);
        }
        else if (user.Age < 30 && existingNote is not null)
        {
            dbContext.SecurityNotes.Remove(existingNote);
        }
    }
}
