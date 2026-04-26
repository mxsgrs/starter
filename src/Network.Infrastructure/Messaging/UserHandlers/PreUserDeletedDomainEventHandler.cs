using Microsoft.EntityFrameworkCore;
using Network.Application.Shared.Events;
using Network.Domain.Aggregates.UserAggregate;
using Network.Infrastructure.Persistance;

namespace Network.Infrastructure.Messaging.UserHandlers;

public class PreUserDeletedDomainEventHandler(UserDbContext dbContext)
    : IPreSavedDomainEventHandler<UserDeletedDomainEvent>
{
    /// <summary>
    /// Write an audit log entry when a user is deleted and remove their security note if one exists — committed atomically
    /// </summary>
    public async Task HandleAsync(UserDeletedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        UserAuditLog auditLog = UserAuditLog.Create(domainEvent.UserId, nameof(UserDeletedDomainEvent));
        await dbContext.AuditLogs.AddAsync(auditLog, cancellationToken);

        SecurityNote? note = await dbContext.SecurityNotes
            .FirstOrDefaultAsync(n => n.UserId == domainEvent.UserId, cancellationToken);

        if (note is not null)
            dbContext.SecurityNotes.Remove(note);
    }
}
