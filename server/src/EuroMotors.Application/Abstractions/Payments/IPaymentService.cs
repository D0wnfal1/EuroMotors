namespace EuroMotors.Application.Abstractions.Payments;

public interface IPaymentService
{
    Task<PaymentResponse> ChargeAsync(decimal amount);

    Task RefundAsync(Guid transactionId, decimal amount);
}
