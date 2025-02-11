using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Categories.Events;

public record CategoryCreatedDomainEvent(Guid CategoryId) : IDomainEvent;
