using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Categories.Events;

public class CategoryIsNotAvailableDomainEvent(Guid categoryId) : IDomainEvent
{
    public Guid CategoryId { get; init; } = categoryId;
}
