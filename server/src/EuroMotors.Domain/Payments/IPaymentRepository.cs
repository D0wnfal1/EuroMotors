using EuroMotors.Domain.Products;

namespace EuroMotors.Domain.Payments;

public interface IPaymentRepository
{
    Task<Payment?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Payment>> GetForProductAsync(Product product, CancellationToken cancellationToken = default);

    void Insert(Payment payment);
}
