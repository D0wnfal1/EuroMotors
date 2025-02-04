using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Todos.Delete;

public sealed record DeleteTodoCommand(Guid TodoItemId) : ICommand;
