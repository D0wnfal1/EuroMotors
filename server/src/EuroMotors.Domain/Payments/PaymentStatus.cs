namespace EuroMotors.Domain.Payments;

public enum PaymentStatus
{
    Pending = 0,
    Error = 1,
    Success = 2,
    Failure = 3,
    Reversed = 4
}
