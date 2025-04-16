namespace EuroMotors.Infrastructure.Callback;

internal sealed class CallbackOptions
{
    public required string BotToken { get; init; }
    public required string ChatId { get; init; }
}
