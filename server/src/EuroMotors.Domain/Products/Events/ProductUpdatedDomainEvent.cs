using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Products.Events;

public sealed record ProductUpdatedDomainEvent(Guid ProductId) : IDomainEvent;
