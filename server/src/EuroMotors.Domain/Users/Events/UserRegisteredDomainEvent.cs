using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Users.Events;

public sealed record UserRegisteredDomainEvent(Guid UserId) : IDomainEvent;
