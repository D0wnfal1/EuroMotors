using EuroMotors.Domain.Orders;
using EuroMotors.Domain.Payments;
using EuroMotors.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EuroMotors.Infrastructure.Repositories;

internal sealed class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Payment>> GetForOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Payments
            .AsNoTracking()
            .Where(p => p.OrderId == order.Id)
            .ToListAsync(cancellationToken);
    }
}

