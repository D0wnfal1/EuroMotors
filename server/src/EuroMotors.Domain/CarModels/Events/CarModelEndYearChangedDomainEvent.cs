using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.CarModels.Events;

public sealed class CarModelEndYearChangedDomainEvent(Guid carModelId, int? endYear) : IDomainEvent
{
    public Guid CarModelId { get; init; } = carModelId;

    public int? EndYear { get; init; } = endYear;
}
