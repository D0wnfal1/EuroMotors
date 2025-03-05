using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.CarModels.Events;

public sealed class CarModelBrandChangedDomainEvent(Guid carModelId, string brand) : IDomainEvent
{
    public Guid CarModelId { get; init; } = carModelId;

    public string Brand { get; init; } = brand;
}
