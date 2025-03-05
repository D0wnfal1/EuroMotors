using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.CarModels.Events;

public sealed class CarModelModelChangedDomainEvent(Guid carModelId, string model) : IDomainEvent
{
    public Guid CarModelId { get; init; } = carModelId;

    public string Model { get; init; } = model;
}
