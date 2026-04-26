using Network.Application.Shared.Events;
using Network.Domain.Aggregates.UserAggregate;
using Network.Infrastructure.Persistance;

namespace Network.Infrastructure.Messaging;

public class UserUpdatedDomainEventHandler(UserDbContext dbContext)
    : IDomainEventHandler<UserUpdatedDomainEvent>
{
    /// <summary>
    /// Write an audit log entry when a user is updated — runs before SaveChangesAsync so it is committed atomically.
    /// </summary>
    public async Task HandleAsync(UserUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        UserAuditLog auditLog = UserAuditLog.Create(domainEvent.UserId, nameof(UserUpdatedDomainEvent));
        await dbContext.AuditLogs.AddAsync(auditLog, cancellationToken);
    }
}
