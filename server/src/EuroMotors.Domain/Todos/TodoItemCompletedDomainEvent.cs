using EuroMotors.SharedKernel;

namespace EuroMotors.Domain.Todos;

public sealed record TodoItemCompletedDomainEvent(Guid TodoItemId) : IDomainEvent;
