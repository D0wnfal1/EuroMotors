using EuroMotors.Application.Abstractions.Payments;

namespace EuroMotors.Infrastructure.Payments;

internal sealed class PaymentService : IPaymentService
{
    public async Task<PaymentResponse> ChargeAsync(decimal amount)
    {
        // Here is the code for processing payment via API (LiqPay)
        await Task.Delay(500); // Simulate a request to a payment gateway
        return new PaymentResponse(Guid.NewGuid(), amount);
    }

    public async Task RefundAsync(Guid transactionId, decimal amount)
    {
        // Here is the code to return money through the payment gateway
        await Task.Delay(500); // Simulate a request to a payment gateway
    }
}
