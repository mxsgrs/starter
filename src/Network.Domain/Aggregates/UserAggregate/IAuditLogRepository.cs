namespace Network.Domain.Aggregates.UserAggregate;

public interface IAuditLogRepository
{
    /// <summary>
    /// Stage a new audit log entry to be committed as part of the current transaction
    /// </summary>
    Task AddAsync(UserAuditLog auditLog, CancellationToken cancellationToken);
}
