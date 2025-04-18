using System.Text.Json.Serialization;

namespace EuroMotors.Api.Controllers.Products;

public sealed class ProductRequest()
{
    public string Name { get; set; }
    public List<SpecificationRequest> Specifications { get; set; }
    public string VendorCode { get; set; }
    [JsonRequired]
    public Guid CategoryId { get; set; }
    [JsonRequired]
    public Guid CarModelId { get; set; }
    [JsonRequired]
    public decimal Price { get; set; }
    [JsonRequired]
    public decimal Discount { get; set; }
    [JsonRequired]
    public int Stock { get; set; }
}
