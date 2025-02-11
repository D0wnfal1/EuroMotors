using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Products.Events;

public sealed record ProductCreatedDomainEvent(Guid ProductId) : IDomainEvent;
