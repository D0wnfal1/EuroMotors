using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Payments;

namespace EuroMotors.Application.Abstractions.Payments;

public interface IPaymentService
{
    Task<string> CreatePaymentAsync(Payment payment);
}
