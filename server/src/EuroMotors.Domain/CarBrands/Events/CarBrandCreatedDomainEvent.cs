using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.CarBrands.Events;

public sealed record CarBrandCreatedDomainEvent(Guid CarBrandId) : IDomainEvent;
