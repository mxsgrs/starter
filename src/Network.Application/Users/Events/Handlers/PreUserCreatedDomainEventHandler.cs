using Network.Application.Shared.Events;

namespace Network.Application.Users.Events.Handlers;

public class PreUserCreatedDomainEventHandler(
    IUserRepository userRepository,
    IAuditLogRepository auditLogRepository)
    : IPreSavedDomainEventHandler<UserCreatedDomainEvent>
{
    /// <summary>
    /// Write an audit log entry and, when the new user is 30 or older, a security note — both committed atomically with the user insert
    /// </summary>
    public async Task HandleAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        Result<AuditLog> auditLogResult = AuditLog.Create(domainEvent.UserId, nameof(UserCreatedDomainEvent));
        if (auditLogResult.IsSuccess)
            await auditLogRepository.AddAsync(auditLogResult.Value);

        Result<User> userResult = await userRepository.FindByIdAsync(domainEvent.UserId);
        if (userResult.IsSuccess && userResult.Value.Age >= 30)
        {
            Result<SecurityNote> noteResult = SecurityNote.Create(domainEvent.UserId, "User age is 30 or above");
            if (noteResult.IsSuccess)
                await userRepository.AddSecurityNoteAsync(noteResult.Value);
        }
    }
}
