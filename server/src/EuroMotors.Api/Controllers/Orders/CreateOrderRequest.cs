using EuroMotors.Domain.Orders;
using Newtonsoft.Json;

namespace EuroMotors.Api.Controllers.Orders;

public class CreateOrderRequest
{
    [JsonRequired]
    public Guid CartId { get; set; }
    public Guid? UserId { get; set; }
    public string BuyerName { get;  set; }
    public string BuyerPhoneNumber { get;  set; }
    public string BuyerEmail { get;  set; }
    [JsonRequired]
    public DeliveryMethod DeliveryMethod { get; set; }
    public string? ShippingAddress { get; set; }
    [JsonRequired]
    public PaymentMethod PaymentMethod { get; set; }
}
