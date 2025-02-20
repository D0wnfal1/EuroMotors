using EuroMotors.Domain.Payments;

namespace EuroMotors.Application.Payments;

public sealed record PaymentCallbackResponse(Guid PaymentId, Guid OrderId, string Status, decimal Amount);

