using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Payments.GetPaymentById;
using EuroMotors.Domain.Payments;

namespace EuroMotors.Application.Payments.GetPaymentByStatus;

public sealed record GetPaymentByStatusQuery(PaymentStatus Status) : IQuery<PaymentResponse>;
