using EuroMotors.SharedKernel;

namespace EuroMotors.Domain.Todos;

public sealed record TodoItemCreatedDomainEvent(Guid TodoItemId) : IDomainEvent;
