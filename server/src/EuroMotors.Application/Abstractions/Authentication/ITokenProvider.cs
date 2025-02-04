using EuroMotors.Domain.Users;

namespace EuroMotors.Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string Create(User user);
}
