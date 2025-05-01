using Newtonsoft.Json;

namespace EuroMotors.Application.Payments;
public sealed class PaymentCallbackResponse
{
    [JsonProperty("order_id")]
    public string OrderId { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("amount")]
    public decimal Amount { get; set; }
}

