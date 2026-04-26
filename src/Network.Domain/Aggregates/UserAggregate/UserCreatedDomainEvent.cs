using Network.Domain.Events;

namespace Network.Domain.Aggregates.UserAggregate;

public record UserCreatedDomainEvent(Guid UserId) : DomainEvent;
