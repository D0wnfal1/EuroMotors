using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.CarBrands.Events;

public sealed record CarBrandNameChangedDomainEvent(Guid CarBrandId, string Name) : IDomainEvent;