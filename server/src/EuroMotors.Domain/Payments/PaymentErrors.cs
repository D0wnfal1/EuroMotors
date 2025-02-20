using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Payments;

public static class PaymentErrors
{
    public static Error NotFound(Guid paymentId) =>
        Error.NotFound("Payments.NotFound", $"The payment with the identifier {paymentId} was not found");

    public static Error NotFoundPaymentStatus(PaymentStatus status) =>
        Error.NotFound("Payments.NotFoundPaymentStatus", $"The payment with the status {status} was not found");

    public static readonly Error AlreadyRefunded =
        Error.Problem("Payments.AlreadyRefunded", "The payment was already refunded");

    public static readonly Error NotEnoughFunds =
        Error.Problem("Payments.NotEnoughFunds", "There are not enough funds for a refund");

    public static Error InvalidData() =>
        Error.Failure("Payments.InvalidData", "Invalid data.");

    public static Error InvalidResponse() =>
        Error.Failure("Payments.InvalidResponse", "Invalid LiqPay response.");
}
