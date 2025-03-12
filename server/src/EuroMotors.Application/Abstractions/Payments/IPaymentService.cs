using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Payments;

namespace EuroMotors.Application.Abstractions.Payments;

public interface IPaymentService
{
    Task<Dictionary<string, string>> CreatePaymentAsync(Payment payment);
    Task<Result> ProcessPaymentCallbackAsync(string data, string signature);
}
