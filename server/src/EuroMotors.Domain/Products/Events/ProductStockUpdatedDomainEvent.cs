using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Products.Events;

public sealed record ProductStockUpdatedDomainEvent(Guid ProductId, int Stock) : IDomainEvent;
