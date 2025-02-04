using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Todos.Complete;

public sealed record CompleteTodoCommand(Guid TodoItemId) : ICommand;
