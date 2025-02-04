using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Users.Login;

public sealed record LoginUserCommand(string Email, string Password) : ICommand<string>;
