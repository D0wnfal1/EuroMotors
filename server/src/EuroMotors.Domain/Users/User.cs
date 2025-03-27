using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Users.Events;

namespace EuroMotors.Domain.Users;

public sealed class User : Entity
{
    private readonly List<Role> _roles = [];
    private User()
    {

    }

    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PasswordHash { get; set; }

    public string? PhoneNumber { get; private set; }
    public string? City { get; private set; }

    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }

    public IReadOnlyCollection<Role> Roles => _roles.ToList();

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

        user._roles.Add(Role.Customer);

        return user;
    }

    public void UpdateContactInfo(string email, string firstName, string lastName, string phoneNumber, string city)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        City = city;
    }

    public void SetRefreshToken(string refreshToken, DateTime expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
    }
}
