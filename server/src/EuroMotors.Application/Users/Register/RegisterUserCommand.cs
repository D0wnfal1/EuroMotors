using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Users.Register;

public sealed record RegisterUserCommand(string Email, string FirstName, string LastName, string Password)
    : ICommand<Guid>;
