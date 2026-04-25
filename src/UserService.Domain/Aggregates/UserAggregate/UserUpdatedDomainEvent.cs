using UserService.Domain.Events;

namespace UserService.Domain.Aggregates.UserAggregate;

public record UserUpdatedDomainEvent(Guid UserId) : DomainEvent;
