namespace Network.Domain.Aggregates.UserAggregate;

public record UserUpdatedDomainEvent(Guid UserId) : DomainEvent;
