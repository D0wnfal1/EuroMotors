using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Users.GetByEmail;

public sealed record GetUserByEmailQuery(string Email) : IQuery<UserResponse>;
