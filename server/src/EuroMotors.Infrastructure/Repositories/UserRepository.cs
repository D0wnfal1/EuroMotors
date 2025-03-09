using EuroMotors.Domain.Users;
using EuroMotors.Infrastructure.Database;

namespace EuroMotors.Infrastructure.Repositories;

internal sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public override void Insert(User user)
    {
        foreach (Role role in user.Roles)
        {
            _dbContext.Attach(role);
        }

        _dbContext.Add(user);
    }
}
