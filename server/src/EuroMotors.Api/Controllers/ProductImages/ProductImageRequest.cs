using Newtonsoft.Json;

namespace EuroMotors.Api.Controllers.ProductImages;

public sealed class ProductImageRequest
{
    [JsonRequired]
    public Uri Url { get; set; }
    [JsonRequired]
    public Guid ProductId { get; set; }
}
