namespace EuroMotors.Application.Users.GetByEmail;

public sealed record UserResponse
{
    public Guid Id { get; init; }

    public string Email { get; init; }

    public string FirstName { get; init; }

    public string LastName { get; init; }

    public string? PhoneNumber { get; init; }

    public string? City { get; init; }

    public List<string> Roles { get; init; }
}
