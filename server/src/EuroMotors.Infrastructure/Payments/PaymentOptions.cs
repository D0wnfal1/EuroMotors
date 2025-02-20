namespace EuroMotors.Infrastructure.Payments;

internal sealed class PaymentOptions
{
    public required string PublicKey { get; init; }
    public required string PrivateKey { get; init; }
    public required string ApiVersion { get; init; }
    public required string CallbackUrl { get; init; }
    public required string ResultUrl { get; init; }
}
