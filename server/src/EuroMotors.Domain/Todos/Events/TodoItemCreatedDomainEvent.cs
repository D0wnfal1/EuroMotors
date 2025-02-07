using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Todos.Events;

public sealed record TodoItemCreatedDomainEvent(Guid TodoItemId) : IDomainEvent;
