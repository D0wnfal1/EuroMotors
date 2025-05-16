namespace EuroMotors.Infrastructure.Callback;

internal sealed class CallbackOptions
{
    public required string BotToken { get; init; }
    public required List<string> ChatIds { get; init; } = new();
}
