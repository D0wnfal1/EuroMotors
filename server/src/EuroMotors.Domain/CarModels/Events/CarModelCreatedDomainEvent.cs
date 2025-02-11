using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.CarModels.Events;

public record CarModelCreatedDomainEvent(Guid CarModelId) : IDomainEvent;
