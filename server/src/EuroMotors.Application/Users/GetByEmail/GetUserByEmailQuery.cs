using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Users.GetById;

namespace EuroMotors.Application.Users.GetByEmail;

public sealed record GetUserByEmailQuery(string Email) : IQuery<UserResponse>;
