
using EuroMotors.Domain.Orders;

namespace EuroMotors.Domain.Payments;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Payment>> GetForOrderAsync(Order order, CancellationToken cancellationToken = default);

    void Insert(Payment payment);

    void Update(Payment payment);
}
