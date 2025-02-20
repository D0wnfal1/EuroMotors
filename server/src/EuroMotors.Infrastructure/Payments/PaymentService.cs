using System.Security.Cryptography;
using System.Text;
using EuroMotors.Application.Abstractions.Payments;
using EuroMotors.Domain.Payments;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EuroMotors.Infrastructure.Payments;

internal sealed class PaymentService(IOptions<PaymentOptions> paymentOptions) : IPaymentService
{
    private readonly PaymentOptions _options = paymentOptions.Value;

    public Task<string> CreatePaymentAsync(Payment payment)
    {
        var data = new
        {
            version = _options.ApiVersion,
            public_key = _options.PublicKey,
            action = "pay",
            amount = payment.Amount,
            currency = "UAH",
            description = $"Payment for order {payment.OrderId}",
            order_id = payment.Id.ToString(),
            result_url = _options.ResultUrl,
            server_url = _options.CallbackUrl,
        };

        string jsonData = JsonConvert.SerializeObject(data);
        string encodedData = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonData));
        string signature = GenerateSignature(encodedData);

        return Task.FromResult($"https://www.liqpay.ua/api/3/checkout?data={encodedData}&signature={signature}");
    }

    private string GenerateSignature(string data)
    {
        string toSign = _options.PrivateKey + data + _options.PrivateKey;

#pragma warning disable CA5350
        using var sha1 = SHA1.Create();
#pragma warning restore CA5350
#pragma warning disable CA1850
        byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(toSign));
#pragma warning restore CA1850

        return Convert.ToBase64String(hash);
    }
}
