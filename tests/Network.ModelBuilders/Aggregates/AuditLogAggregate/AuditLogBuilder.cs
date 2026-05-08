using FluentResults;
using Network.Domain.Aggregates.AuditLogAggregate;
using Network.ModelBuilders.Shared;

namespace Network.ModelBuilders.Aggregates.AuditLogAggregate;

public class AuditLogBuilder : IEntityModelBuilder<AuditLog>
{
    private Guid _userId = Guid.NewGuid();
    private AuditLogEventType _eventType = AuditLogEventType.UserCreated;

    public AuditLogBuilder WithUserId(Guid v) { _userId = v; return this; }
    public AuditLogBuilder WithEventType(AuditLogEventType v) { _eventType = v; return this; }

    /// <summary>
    /// Build and return the Result produced by the factory method.
    /// </summary>
    public Result<AuditLog> BuildResult() => AuditLog.Create(_userId, _eventType);

    /// <summary>
    /// Build an AuditLog instance using the domain factory method.
    /// </summary>
    public AuditLog Build() => AuditLog.Create(_userId, _eventType).Value;
}
