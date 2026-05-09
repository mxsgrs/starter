using Network.Application.Shared.Events;

namespace Network.Application.Users.Events.Handlers;

public class PreUserCreatedDomainEventHandler(
    IAuditLogRepository auditLogRepository)
    : IPreSavedDomainEventHandler<UserCreatedDomainEvent>
{
    /// <summary>
    /// Write an audit log entry committed atomically with the user insert
    /// </summary>
    public async Task HandleAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        Result<AuditLog> auditLogResult = AuditLog.Create(domainEvent.UserId, AuditLogEventType.UserCreated);
        if (auditLogResult.IsSuccess)
            await auditLogRepository.AddAsync(auditLogResult.Value);
    }
}
