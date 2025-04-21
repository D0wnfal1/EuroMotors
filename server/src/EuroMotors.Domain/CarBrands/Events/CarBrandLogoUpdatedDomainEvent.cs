using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.CarBrands.Events;

public sealed record CarBrandLogoUpdatedDomainEvent(Guid CarBrandId) : IDomainEvent;