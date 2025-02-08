using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Products.Events;

public record ProductStockUpdatedEvent(Guid ProductId, int Stock) : IDomainEvent;
