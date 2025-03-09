namespace EuroMotors.Domain.Users;

public sealed class Role
{
    public static readonly Role Admin = new(1, "Admin");
    public static readonly Role Customer = new(2, "Customer");

    private Role(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; init; }

    public string Name { get; init; }

    public ICollection<User> Users { get; init; } = new List<User>();

}
