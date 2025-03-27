using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Users.Login;

namespace EuroMotors.Application.Users.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<AuthenticationResponse>;
