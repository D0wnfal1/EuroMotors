namespace EuroMotors.Domain.Payments;

public enum PaymentStatus
{
    Panding = 0,
    Error = 1,
    Success = 2,
    Failure = 3,
    Reversed = 4
}
