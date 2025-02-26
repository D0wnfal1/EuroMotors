using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.CarModels.Events;

public sealed class CarModelImageUpdatedDomainEvent(Guid carModelId) : IDomainEvent
{
    public Guid CarModelId { get; init; } = carModelId;

}
