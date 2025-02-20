using EuroMotors.Domain.Payments;

namespace EuroMotors.Application.Payments.GetPaymentById;

public sealed record PaymentResponse(Guid Id, Guid OrderId, Guid TransactionId, PaymentStatus Status, decimal Amount, DateTime CreatedAtUtc);
