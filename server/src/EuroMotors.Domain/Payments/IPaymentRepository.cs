namespace EuroMotors.Domain.Payments;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Payment> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    void Insert(Payment payment);

    void Update(Payment payment);
}
