﻿using EuroMotors.Application.Abstractions.Callback;
using Microsoft.Extensions.Options;

namespace EuroMotors.Infrastructure.Callback;

internal sealed class CallbackService(IOptions<CallbackOptions> callbackOptions, HttpClient httpClient) : ICallbackService
{
    private readonly CallbackOptions _options = callbackOptions.Value;

    public async Task SendMessageAsync(string name, string phone)
    {
        string message = $"📞 Новий запит на дзвінок:\nІм'я: {name}\nНомер телефону: {phone}";
        string url = $"https://api.telegram.org/bot{_options.BotToken}/sendMessage";

        foreach (string chatId in _options.ChatIds)
        {
            var payload = new Dictionary<string, string>
            {
                { "chat_id", chatId },
                { "text", message }
            };

            using var content = new FormUrlEncodedContent(payload);
            await httpClient.PostAsync(url, content);
        }
    }

    public async Task SendOrderNotificationAsync(Guid orderId, string buyerName, string buyerPhone, string? buyerEmail, decimal orderTotal)
    {
        string message = $"🛒 Нове замовлення #{orderId}:\n" +
                         $"Покупець: {buyerName}\n" +
                         $"Телефон: {buyerPhone}\n" +
                         $"Email: {buyerEmail ?? "не вказано"}\n" +
                         $"Сума: {orderTotal} грн.";

        string url = $"https://api.telegram.org/bot{_options.BotToken}/sendMessage";

        foreach (string chatId in _options.ChatIds)
        {
            var payload = new Dictionary<string, string>
            {
                { "chat_id", chatId },
                { "text", message }
            };

            using var content = new FormUrlEncodedContent(payload);
            await httpClient.PostAsync(url, content);
        }
    }
}
