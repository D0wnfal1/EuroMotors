using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Payments;

namespace EuroMotors.Application.Orders.SetPayment;

public sealed record SetPaymentCommand(Guid OrderId, Guid PaymentId, PaymentStatus PaymentStatus) : ICommand;
