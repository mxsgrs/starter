namespace Network.Domain.Aggregates.UserAggregate;

public record UserDeletedDomainEvent(Guid UserId) : DomainEvent;
