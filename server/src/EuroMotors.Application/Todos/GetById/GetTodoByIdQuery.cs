using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Todos.GetById;

public sealed record GetTodoByIdQuery(Guid TodoItemId) : IQuery<TodoResponse>;
