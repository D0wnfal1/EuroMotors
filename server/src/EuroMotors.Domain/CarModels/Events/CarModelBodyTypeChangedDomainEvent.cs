using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.CarModels.Events;

public sealed class CarModelBodyTypeChangedDomainEvent(Guid carModelId, BodyType bodyType) : IDomainEvent
{
    public Guid CarModelId { get; init; } = carModelId;

    public BodyType BodyType { get; init; } = bodyType;
}
