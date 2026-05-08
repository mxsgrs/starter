using Network.Application.Shared.Events;

namespace Network.Application.Users.Events.Handlers;

public class PreUserUpdatedDomainEventHandler(
    IUserRepository userRepository,
    IAuditLogRepository auditLogRepository)
    : IPreSavedDomainEventHandler<UserUpdatedDomainEvent>
{
    /// <summary>
    /// Write an audit log entry when a user is updated, and create or delete their security note based on age — committed atomically
    /// </summary>
    public async Task HandleAsync(UserUpdatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        Result<AuditLog> auditLogResult = AuditLog.Create(domainEvent.UserId, AuditLogEventType.UserUpdated);
        if (auditLogResult.IsSuccess)
            await auditLogRepository.AddAsync(auditLogResult.Value);

        Result<User> userResult = await userRepository.FindByIdAsync(domainEvent.UserId);
        if (!userResult.IsSuccess) return;

        User user = userResult.Value;
        SecurityNote? existingNote = await userRepository.FindSecurityNoteByUserIdAsync(domainEvent.UserId);

        if (user.Age >= 30 && existingNote is null)
        {
            Result<SecurityNote> noteResult = SecurityNote.Create(domainEvent.UserId, "User age is 30 or above");
            if (noteResult.IsSuccess)
                await userRepository.AddSecurityNoteAsync(noteResult.Value);
        }
        else if (user.Age < 30 && existingNote is not null)
        {
            userRepository.RemoveSecurityNote(existingNote);
        }
    }
}
