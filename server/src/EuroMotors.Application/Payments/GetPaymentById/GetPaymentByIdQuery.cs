
using EuroMotors.Application.Abstractions.Messaging;

namespace EuroMotors.Application.Payments.GetPaymentById;

public sealed record GetPaymentByIdQuery(Guid PaymentId) : IQuery<PaymentResponse>;
