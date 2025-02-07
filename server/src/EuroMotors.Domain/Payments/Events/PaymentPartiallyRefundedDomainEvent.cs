using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Payments.Events;

public sealed class PaymentPartiallyRefundedDomainEvent(Guid paymentId, Guid transactionId, decimal refundAmount) : IDomainEvent
{
    public Guid PaymentId { get; init; } = paymentId;

    public Guid TransactionId { get; init; } = transactionId;

    public decimal RefundAmount { get; init; } = refundAmount;
}
