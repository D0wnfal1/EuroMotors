using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Todos.Events;

public sealed record TodoItemDeletedDomainEvent(Guid TodoItemId) : IDomainEvent;
