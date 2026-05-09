using Network.Application.Shared.Events;
using Network.Domain.Aggregates.FinancialProfileAggregate;

namespace Network.Application.Users.Events.Handlers;

public class PreUserCreatedDomainEventHandler(
    IAuditLogRepository auditLogRepository,
    IFinancialProfileRepository financialProfileRepository)
    : IPreSavedDomainEventHandler<UserCreatedDomainEvent>
{
    /// <summary>
    /// Write an audit log entry and create a financial profile, both committed atomically with the user insert.
    /// </summary>
    public async Task HandleAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        Result<AuditLog> auditLogResult = AuditLog.Create(domainEvent.UserId, AuditLogEventType.UserCreated);
        if (auditLogResult.IsSuccess)
            await auditLogRepository.AddAsync(auditLogResult.Value);

        Result<FinancialProfile> profileResult = FinancialProfile.Create(domainEvent.UserId);
        if (profileResult.IsSuccess)
            await financialProfileRepository.AddAsync(profileResult.Value);
    }
}
