using Network.Application.Shared.Events;

namespace Network.Application.Users.Events.Handlers;

public class PreUserDeletedDomainEventHandler(
    IUserRepository userRepository,
    IAuditLogRepository auditLogRepository)
    : IPreSavedDomainEventHandler<UserDeletedDomainEvent>
{
    /// <summary>
    /// Write an audit log entry when a user is deleted and remove their security note if one exists — committed atomically
    /// </summary>
    public async Task HandleAsync(UserDeletedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        Result<AuditLog> auditLogResult = AuditLog.Create(domainEvent.UserId, nameof(UserDeletedDomainEvent));
        if (auditLogResult.IsSuccess)
            await auditLogRepository.AddAsync(auditLogResult.Value);

        SecurityNote? note = await userRepository.FindSecurityNoteByUserIdAsync(domainEvent.UserId);
        if (note is not null)
            userRepository.RemoveSecurityNote(note);
    }
}
