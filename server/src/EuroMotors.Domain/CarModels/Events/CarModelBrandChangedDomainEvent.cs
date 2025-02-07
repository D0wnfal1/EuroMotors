using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Category;
using EuroMotors.Domain.Products;

namespace EuroMotors.Domain.Brand.Events;

public class CarModelBrandChangedDomainEvent(Guid carModelId, string brand) : IDomainEvent
{
    public Guid CarModelId { get; init; } = carModelId;

    public string Brand { get; init; } = brand;
}
