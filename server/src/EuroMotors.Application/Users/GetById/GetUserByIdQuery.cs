using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Users.GetById;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;
