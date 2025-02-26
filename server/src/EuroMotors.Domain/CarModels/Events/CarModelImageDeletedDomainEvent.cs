using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.CarModels.Events;

public sealed class CarModelImageDeletedDomainEvent(Guid carModelId) : IDomainEvent
{
    public Guid CarModelId { get; init; } = carModelId;
}
