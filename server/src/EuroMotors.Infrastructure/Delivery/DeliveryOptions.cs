namespace EuroMotors.Infrastructure.Delivery;

internal sealed class DeliveryOptions
{
    public required string ApiKey { get; init; }
    public required string ApiUrl { get; init; }
}
