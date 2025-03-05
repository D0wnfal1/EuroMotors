using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Categories.Events;

public sealed record CategoryCreatedDomainEvent(Guid CategoryId) : IDomainEvent;
