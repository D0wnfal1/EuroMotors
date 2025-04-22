using EuroMotors.Domain.CarModels;

namespace EuroMotors.Domain.UnitTests.Products;

internal static class ProductData
{
    public static readonly string Name = new("Test Product");
    public static readonly string VendorCode = new("VENDOR123");
    public static readonly Guid CategoryId = Guid.NewGuid();
    public static readonly IEnumerable<Guid> CarModelIds = new List<Guid>();
    public const decimal Price = 100m;
    public const decimal Discount = 10m;
    public const int Stock = 50;
}
