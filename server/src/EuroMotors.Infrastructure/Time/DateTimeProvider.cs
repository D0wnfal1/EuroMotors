using EuroMotors.Application.Abstractions.Clock;

namespace EuroMotors.Infrastructure.Time;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
