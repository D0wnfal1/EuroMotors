using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Products.Events;

public record ProductUpdatedEvent(Guid ProductId) : IDomainEvent;
