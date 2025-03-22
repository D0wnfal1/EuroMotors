using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Payments.Events;

namespace EuroMotors.Domain.Payments;

public sealed class Payment : Entity
{
    private Payment()
    {

    }

    public Guid OrderId { get; private set; }

    public Guid TransactionId { get; private set; }

    public PaymentStatus Status { get; private set; }

    public decimal Amount { get; private set; }

    public decimal? AmountRefunded { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? RefundedAtUtc { get; set; }

    public static Payment Create(Guid orderId, Guid transactionId, PaymentStatus status, decimal amount)
    {
        var payment = new Payment()
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Status = status,
            TransactionId = transactionId,
            Amount = amount,
            AmountRefunded = 0m,
            CreatedAtUtc = DateTime.UtcNow,
            RefundedAtUtc = null
        };

        payment.RaiseDomainEvent(new PaymentCreatedDomainEvent(payment.Id));

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
            RaiseDomainEvent(new PaymentRefundedDomainEvent(Id, TransactionId, refundAmount));
        }
        else
        {
            RaiseDomainEvent(new PaymentPartiallyRefundedDomainEvent(Id, TransactionId, refundAmount));
        }

        return Result.Success();
    }

    public void ChangeStatus(PaymentStatus status)
    {
        Status = status;
    }
}
