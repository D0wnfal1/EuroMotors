using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Categories.Events;

public sealed class CategoryNameChangedDomainEvent(Guid categoryId, string name) : IDomainEvent
{
    public Guid CategoryId { get; init; } = categoryId;

    public string Name { get; init; } = name;
}
