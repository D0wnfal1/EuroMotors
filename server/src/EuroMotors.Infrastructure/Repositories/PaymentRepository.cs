using EuroMotors.Domain.Payments;
using EuroMotors.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace EuroMotors.Infrastructure.Repositories;

internal sealed class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Payment> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Payment>()
            .FirstOrDefaultAsync(payment => payment.OrderId == orderId, cancellationToken);
    }

    public void Update(Payment payment)
    {
        _dbContext.Update(payment);

    }
}

