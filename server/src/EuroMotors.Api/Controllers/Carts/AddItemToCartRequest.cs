using Newtonsoft.Json;

namespace EuroMotors.Api.Controllers.Carts;

public sealed class AddItemToCartRequest
{
    [JsonRequired]
    public Guid UserId { get; set; }
    [JsonRequired]
    public Guid ProductId { get; set; }
    [JsonRequired]
    public int Quantity { get; set; }
}
