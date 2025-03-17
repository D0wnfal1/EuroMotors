namespace EuroMotors.Application.Abstractions.Authentication;

public interface IUserContext
{
    Guid UserId { get; }

    List<string> Roles { get; }
}
