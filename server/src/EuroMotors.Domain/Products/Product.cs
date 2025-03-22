using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.ProductImages;
using EuroMotors.Domain.Products.Events;

namespace EuroMotors.Domain.Products;

public sealed class Product : Entity
{
    private Product()
    {

    }

    public Guid CategoryId { get; private set; }

    public Guid CarModelId { get; private set; }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public string VendorCode { get; private set; }

    public decimal Price { get; private set; }

    public decimal Discount { get; private set; } = 0m;

    public int Stock { get; private set; }

    public bool IsAvailable { get; private set; }

    public List<ProductImage> Images { get; private set; } = [];

    public static Product Create(
        string name,
        string description,
        string vendorCode,
        Guid categoryId,
        Guid carModelId,
        decimal price,
        decimal discount,
        int stock)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            VendorCode = vendorCode,
            CategoryId = categoryId,
            CarModelId = carModelId,
            Price = price,
            Discount = discount,
            Stock = stock,
            IsAvailable = stock > 0
        };


        product.RaiseDomainEvent(new ProductCreatedDomainEvent(product.Id));

        return product;
    }

    public void Update(string name, string description, decimal price, decimal discount, int stock)
    {
        Name = name;
        Description = description;
        Price = price;
        Discount = discount;
        Stock = stock;
        IsAvailable = stock > 0;

        RaiseDomainEvent(new ProductUpdatedDomainEvent(Id));
    }

    public Result UpdateStock(int stock)
    {
        Stock = stock;

        IsAvailable = stock > 0;

        return Result.Success();
    }

    public Result SubtractProductQuantity(int quantity)
    {
        if (Stock < quantity)
        {
            return Result.Failure(ProductErrors.NotEnoughStock(Stock));
        }

        Stock -= quantity;

        IsAvailable = Stock > 0;

        RaiseDomainEvent(new ProductStockUpdatedDomainEvent(Id, Stock));

        return Result.Success();
    }

    public Result AddProductQuantity(int quantity)
    {
        Stock += quantity;

        RaiseDomainEvent(new ProductStockUpdatedDomainEvent(Id, Stock));

        return Result.Success();
    }

    public void MarkAsNotAvailable()
    {
        if (!IsAvailable)
        {
            return;
        }

        IsAvailable = false;

        RaiseDomainEvent(new ProductIsNotAvailableDomainEvent(Id));
    }
}
