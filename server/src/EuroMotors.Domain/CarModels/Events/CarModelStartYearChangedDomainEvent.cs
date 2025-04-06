using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.CarModels.Events;

public sealed class CarModelStartYearChangedDomainEvent(Guid carModelId, int startYear) : IDomainEvent
{
    public Guid CarModelId { get; init; } = carModelId;

    public int StartYear { get; init; } = startYear;
}
