namespace EuroMotors.SharedKernel;

public interface IDateTimeProvider
{
    public DateTime UtcNow { get; }
}
