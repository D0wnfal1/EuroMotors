using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Payments.GetPaymentById;

namespace EuroMotors.Application.Payments.GetPaymentByOrderId;

public sealed record GetPaymentByOrderIdQuery(Guid OrderId) : IQuery<PaymentResponse>;
