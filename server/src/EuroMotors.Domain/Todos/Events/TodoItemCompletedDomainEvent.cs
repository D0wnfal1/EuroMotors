using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Todos.Events;

public sealed record TodoItemCompletedDomainEvent(Guid TodoItemId) : IDomainEvent;
