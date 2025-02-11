using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Payments.RefundPayment;

public sealed record RefundPaymentCommand(Guid PaymentId, decimal Amount) : ICommand;
