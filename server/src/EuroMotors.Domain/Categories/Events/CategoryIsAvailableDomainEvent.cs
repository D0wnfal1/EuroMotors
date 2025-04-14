using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Categories.Events;

public sealed class CategoryIsAvailableDomainEvent(Guid categoryId) : IDomainEvent
{
    public Guid CategoryId { get; init; } = categoryId;
}
