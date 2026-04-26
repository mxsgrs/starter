using Network.Domain.Events;

namespace Network.Domain.Aggregates.UserAggregate;

public record UserUpdatedDomainEvent(Guid UserId) : DomainEvent;
