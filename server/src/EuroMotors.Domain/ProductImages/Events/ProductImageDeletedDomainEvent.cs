using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.ProductImages.Events;

public sealed class ProductImageDeletedDomainEvent(Guid productImageId) : IDomainEvent
{
    public Guid ProductImageId { get; init; } = productImageId;
}
