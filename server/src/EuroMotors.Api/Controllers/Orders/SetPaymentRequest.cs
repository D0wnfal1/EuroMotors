using EuroMotors.Domain.Payments;
using Newtonsoft.Json;

namespace EuroMotors.Api.Controllers.Orders;

public sealed class SetPaymentRequest
{
    [JsonRequired]
    public Guid PaymentId { get; set; }
    [JsonRequired]
    public PaymentStatus Status { get; set; }
}
