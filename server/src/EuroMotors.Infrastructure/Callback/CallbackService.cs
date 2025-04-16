using EuroMotors.Application.Abstractions.Callback;
using Microsoft.Extensions.Options;

namespace EuroMotors.Infrastructure.Callback;

internal sealed class CallbackService(IOptions<CallbackOptions> callbackOptions, HttpClient httpClient) : ICallbackService
{
    private readonly CallbackOptions _options = callbackOptions.Value;

    public async Task SendMessageAsync(string name, string phone)
    {
        string message = $"📞 New call request:\nName: {name}\nPhone Number: {phone}";
        string url = $"https://api.telegram.org/bot{_options.BotToken}/sendMessage";
        var payload = new Dictionary<string, string>
        {
            { "chat_id", _options.ChatId },
            { "text", message }
        };

        using var content = new FormUrlEncodedContent(payload);
        await httpClient.PostAsync(url, content);
    }
}
