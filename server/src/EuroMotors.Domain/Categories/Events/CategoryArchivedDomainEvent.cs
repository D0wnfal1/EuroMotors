using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Category.Events;

public sealed class CategoryArchivedDomainEvent(Guid categoryId) : IDomainEvent
{
    public Guid CategoryId { get; init; } = categoryId;
}
