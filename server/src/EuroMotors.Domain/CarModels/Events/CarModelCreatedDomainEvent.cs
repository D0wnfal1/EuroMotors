using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.CarModels.Events;

public sealed record CarModelCreatedDomainEvent(Guid CarModelId) : IDomainEvent;
