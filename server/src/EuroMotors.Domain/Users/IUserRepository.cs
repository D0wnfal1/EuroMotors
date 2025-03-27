namespace EuroMotors.Domain.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);

    void Update(User user);

    void Insert(User user);
}
