namespace Network.Domain.Aggregates.AuditLogAggregate;

public class AuditLog : AggregateRoot
{
    /// <summary>
    /// Create a new audit log entry for a user event.
    /// </summary>
    public static Result<AuditLog> Create(Guid userId, AuditLogEventType eventType) =>
        Result.Ok(new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            EventType = eventType,
            OccurredOn = DateTime.UtcNow
        });

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public AuditLogEventType EventType { get; private set; }
    public DateTime OccurredOn { get; private set; }

    private AuditLog() { } // EF Core
}
