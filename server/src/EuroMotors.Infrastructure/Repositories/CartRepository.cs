using EuroMotors.Domain.Carts;
using EuroMotors.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EuroMotors.Infrastructure.Repositories;

internal sealed class CartRepository : Repository<Cart>, ICartRepository
{
    public CartRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Cart>()
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(cart => cart.UserId == userId, cancellationToken);
    }

    public async void Update(Cart cart)
    {
        _dbContext.Set<Cart>().Update(cart);

        await _dbContext.SaveChangesAsync();
    }

}
