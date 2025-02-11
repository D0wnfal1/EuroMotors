using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.ProductImages.Events;

public sealed record ProductImageCreatedDomainEvent(Guid ProductImageId) : IDomainEvent;
