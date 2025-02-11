using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Products.Events;

public sealed record ProductIsNotAvailableDomainEvent(Guid ProductId) : IDomainEvent;

