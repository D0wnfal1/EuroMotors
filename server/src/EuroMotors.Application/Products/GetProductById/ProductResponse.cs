namespace EuroMotors.Application.Products.GetProductById;

public sealed class ProductResponse
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public Guid CarModelId { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string VendorCode { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public int Stock { get; set; }
    public bool IsAvailable { get; set; }
    public string[]? Images { get; set; }

    public ProductResponse() { }

}
