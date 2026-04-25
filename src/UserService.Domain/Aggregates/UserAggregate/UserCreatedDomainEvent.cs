using UserService.Domain.Events;

namespace UserService.Domain.Aggregates.UserAggregate;

public record UserCreatedDomainEvent(Guid UserId) : DomainEvent;
