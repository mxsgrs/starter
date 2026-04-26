using Network.Application.Shared.Events;
using Network.Domain.Aggregates.UserAggregate;
using Network.Infrastructure.Persistance;

namespace Network.Infrastructure.Messaging;

public class UserDeletedDomainEventHandler(UserDbContext dbContext)
    : IDomainEventHandler<UserDeletedDomainEvent>
{
    /// <summary>
    /// Write an audit log entry when a user is deleted — runs before SaveChangesAsync so it is committed atomically.
    /// </summary>
    public async Task HandleAsync(UserDeletedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        UserAuditLog auditLog = UserAuditLog.Create(domainEvent.UserId, nameof(UserDeletedDomainEvent));
        await dbContext.AuditLogs.AddAsync(auditLog, cancellationToken);
    }
}
