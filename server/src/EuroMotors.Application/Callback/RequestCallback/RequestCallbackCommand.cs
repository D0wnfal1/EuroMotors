using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Callback.RequestCallback;

public sealed record RequestCallbackCommand(string Name, string Phone) : ICommand;
