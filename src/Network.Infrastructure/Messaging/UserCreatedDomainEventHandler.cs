using Network.Application.Shared.Events;
using Network.Domain.Aggregates.UserAggregate;
using Network.Infrastructure.Persistance;

namespace Network.Infrastructure.Messaging;

public class UserCreatedDomainEventHandler(UserDbContext dbContext)
    : IDomainEventHandler<UserCreatedDomainEvent>
{
    /// <summary>
    /// Write an audit log entry when a user is created — runs before SaveChangesAsync so it is committed atomically
    /// </summary>
    public async Task HandleAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        UserAuditLog auditLog = UserAuditLog.Create(domainEvent.UserId, nameof(UserCreatedDomainEvent));
        await dbContext.AuditLogs.AddAsync(auditLog, cancellationToken);
    }
}
