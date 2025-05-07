namespace EuroMotors.Application.Products.ImportProducts;

public sealed record ImportProductRequest
{
    public string Name { get; init; }
    public string VendorCode { get; init; }
    public string CategoryName { get; init; }
    public string CarModelNames { get; init; }
    public decimal Price { get; init; }
    public decimal Discount { get; init; }
    public int Stock { get; init; }
    public List<ImportProductSpecificationRequest> Specifications { get; init; }
}

public sealed record ImportProductSpecificationRequest
{
    public string SpecificationName { get; init; }
    public string SpecificationValue { get; init; }
}


