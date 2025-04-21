using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.CarModels.Events;

public sealed record CarModelModelChangedDomainEvent(Guid CarModelId, string ModelName) : IDomainEvent;
