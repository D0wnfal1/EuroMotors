using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Category.Events;

public record CategoryCreatedDomainEvent(Guid categoryId) : IDomainEvent;
