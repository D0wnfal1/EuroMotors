using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Products.Events;

public sealed record ProductIsAvailableDomainEvent(Guid ProductId) : IDomainEvent;

