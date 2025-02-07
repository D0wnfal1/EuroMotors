using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Brand.Events;

public record CarModelCreatedDomainEvent(Guid carModelId) : IDomainEvent;
