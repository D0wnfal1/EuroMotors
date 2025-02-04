using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Todos.Get;

public sealed record GetTodosQuery(Guid UserId) : IQuery<List<TodoResponse>>;
