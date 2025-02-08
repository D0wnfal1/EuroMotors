using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Users.Events;

namespace EuroMotors.Domain.Users;

public sealed class User : Entity
{
    private User()
    {

    }

    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PasswordHash { get; set; }

    public static User Create(string email, string firstName, string lastName, string passwordHash)
    {
        var user = new User()
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PasswordHash = passwordHash
        };

        user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id));

        return user;
    }
}
