using System.Text.Json.Serialization;

namespace EuroMotors.Api.Controllers.Products;

public sealed class CreateProductRequest()
{
    public string Name { get; set; }
    public List<SpecificationRequest> Specifications { get; set; }
    public string VendorCode { get; set; }
    [JsonRequired]
    public Guid CategoryId { get; set; }
    [JsonRequired]
    public List<Guid> CarModelIds { get; set; }
    [JsonRequired]
    public decimal Price { get; set; }
    [JsonRequired]
    public decimal Discount { get; set; }
    [JsonRequired]
    public int Stock { get; set; }
}
