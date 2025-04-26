using EuroMotors.Application.Users.GetByEmail;

namespace EuroMotors.Api.Controllers.Users;

public sealed record AuthState
{
    public required bool IsAuthenticated { get; init; }
    public UserResponse? User { get; init; }
}
