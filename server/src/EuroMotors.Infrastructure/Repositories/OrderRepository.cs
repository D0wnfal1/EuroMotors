using EuroMotors.Domain.Orders;
using EuroMotors.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EuroMotors.Infrastructure.Repositories;

internal sealed class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Order?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Order>()
            .Include(c => c.OrderItems)
            .FirstOrDefaultAsync(order => order.UserId == userId, cancellationToken);
    }

    public async Task AddItemsToOrder(Order order, CancellationToken cancellationToken = default)
    {
        await _dbContext.OrderItems.AddRangeAsync(order.OrderItems, cancellationToken);
    }

    public async Task Delete(Guid orderId, CancellationToken cancellationToken = default)
    {
        Order? order = await _dbContext.Orders.Include(c => c.OrderItems)
            .FirstOrDefaultAsync(c => c.Id == orderId, cancellationToken);

        if (order == null)
        {
            return;
        }

        _dbContext.OrderItems.RemoveRange(order.OrderItems);

        _dbContext.Remove(order);

    }
}
