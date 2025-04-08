using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.CarModels.Events;

public sealed record CarModelEngineSpecUpdatedDomainEvent(Guid CarModelId) : IDomainEvent;
