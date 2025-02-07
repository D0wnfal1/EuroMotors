using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Orders;
using EuroMotors.Domain.Payments.Events;

namespace EuroMotors.Domain.Payments;

public sealed class Payment : Entity
{
    public Payment(Guid id, Guid orderId, Guid transactionId, decimal amount, decimal? amountRefunded, DateTime createdAtUtc, DateTime? refundedAtUtc)
        : base(id)
    {
        OrderId = orderId;
        TransactionId = transactionId;
        Amount = amount;
        AmountRefunded = amountRefunded ?? 0m;
        CreatedAtUtc = createdAtUtc;
        RefundedAtUtc = refundedAtUtc;
    }

    public Guid OrderId { get; private set; }

    public Guid TransactionId { get; private set; }

    public decimal Amount { get; private set; }

    public decimal? AmountRefunded { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? RefundedAtUtc { get; set; }

    public static Payment Create(Order order, Guid transactionId, decimal amount, string currency)
    {
        var payment = new Payment(
            Guid.NewGuid(),
            order.Id,
            transactionId,
            amount,
            0m,
            DateTime.UtcNow,
            null
        );

        payment.RaiseDomainEvents(new PaymentCreatedDomainEvent(payment.Id));

        return payment;
    }

    public Result Refund(decimal refundAmount)
    {
        if (AmountRefunded == Amount)
        {
            return Result.Failure(PaymentErrors.AlreadyRefunded);
        }

        if (AmountRefunded + refundAmount > Amount)
        {
            return Result.Failure(PaymentErrors.NotEnoughFunds);
        }

        AmountRefunded += refundAmount;

        if (Amount == AmountRefunded)
        {
            RaiseDomainEvents(new PaymentRefundedDomainEvent(Id, TransactionId, refundAmount));
        }
        else
        {
            RaiseDomainEvents(new PaymentPartiallyRefundedDomainEvent(Id, TransactionId, refundAmount));
        }

        return Result.Success();
    }
}
