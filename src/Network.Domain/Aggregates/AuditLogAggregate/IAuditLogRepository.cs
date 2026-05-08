namespace Network.Domain.Aggregates.AuditLogAggregate;

public interface IAuditLogRepository
{
    /// <summary>
    /// Stage a new audit log entry to be committed as part of the current transaction
    /// </summary>
    Task AddAsync(AuditLog auditLog);
}
