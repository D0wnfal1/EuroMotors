using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Users.Events;

namespace EuroMotors.Domain.Users;

public sealed class User : Entity
{
    private User(Guid id, string email, string firstName, string lastName, string passwordHash) : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PasswordHash = passwordHash;
    }

    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PasswordHash { get; set; }

    public static User Create(string email, string firstName, string lastName, string passwordHash)
    {
        var user = new User(Guid.NewGuid(), email, firstName, lastName, passwordHash);

        user.RaiseDomainEvents(new UserRegisteredDomainEvent(user.Id));

        return new User(Guid.NewGuid(), email, firstName, lastName, passwordHash);
    }
}
