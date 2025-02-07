using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Payments.Events;

public sealed class PaymentCreatedDomainEvent(Guid paymentId) : IDomainEvent
{
    public Guid PaymentId { get; init; } = paymentId;
}
