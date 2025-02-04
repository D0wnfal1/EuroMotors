using EuroMotors.SharedKernel;

namespace EuroMotors.Domain.Todos;

public sealed record TodoItemDeletedDomainEvent(Guid TodoItemId) : IDomainEvent;
