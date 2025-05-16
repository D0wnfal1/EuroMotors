using System.Security.Cryptography;
using System.Text;
using EuroMotors.Application.Abstractions.Payments;
using EuroMotors.Application.Payments;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Orders;
using EuroMotors.Domain.Payments;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EuroMotors.Infrastructure.Payments;

internal sealed class PaymentService(
    IOptions<PaymentOptions> paymentOptions,
    IPaymentRepository paymentRepository,
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork) : IPaymentService
{
    private readonly PaymentOptions _options = paymentOptions.Value;

    public async Task<Dictionary<string, string>> CreatePaymentAsync(Payment payment)
    {
        Order? order = await orderRepository.GetByIdAsync(payment.OrderId);
        
        if (order == null)
        {
            return await Task.FromResult(new Dictionary<string, string>());
        }
        
        string description = $"Замовлення №{payment.OrderId.ToString().Substring(0, 8)}... ";
        
        var data = new
        {
            version = _options.ApiVersion,
            public_key = _options.PublicKey,
            action = "pay",
            amount = payment.Amount,
            currency = "UAH",
            description,
            order_id = payment.OrderId,
            result_url = _options.ResultUrl + $"/{payment.OrderId}",
            server_url = _options.CallbackUrl,
        };

        string jsonData = JsonConvert.SerializeObject(data);
        string encodedData = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonData));
        string signature = GenerateSignature(encodedData);

        var result = new Dictionary<string, string>
        {
            { "data", encodedData },
            { "signature", signature }
        };

        return await Task.FromResult(result);
    }

    public async Task<Result> ProcessPaymentCallbackAsync(string data, string signature)
    {
        string computedSignature = GenerateSignature(data);
        if (computedSignature != signature)
        {
            return Result.Failure(PaymentErrors.InvalidData());
        }

        string decodedData = Encoding.UTF8.GetString(Convert.FromBase64String(data));

        PaymentCallbackResponse? paymentResponse = JsonConvert.DeserializeObject<PaymentCallbackResponse>(decodedData);

        if (paymentResponse == null)
        {
            return Result.Failure(PaymentErrors.InvalidData());
        }

        Payment? payment = await paymentRepository.GetByOrderIdAsync(Guid.Parse(paymentResponse.OrderId));
        if (payment == null)
        {
            return Result.Failure(PaymentErrors.NotFound(Guid.Parse(paymentResponse.OrderId)));
        }

        payment.ChangeStatus(paymentResponse.Status switch
        {
            "success" => PaymentStatus.Success,
            "failure" => PaymentStatus.Failure,
            "error" => PaymentStatus.Error,
            _ => PaymentStatus.Pending
        });

        if (payment.Status == PaymentStatus.Success)
        {
            Order? order = await orderRepository.GetByIdAsync(payment.OrderId);
            order?.ChangeStatus(OrderStatus.Paid);
        }

        await unitOfWork.SaveChangesAsync();

        return Result.Success();
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
