using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.CarModels.Events;

public sealed record CarModelEngineSpecChangedDomainEvent(Guid CarModelId, EngineSpec NewSpec) : IDomainEvent;

