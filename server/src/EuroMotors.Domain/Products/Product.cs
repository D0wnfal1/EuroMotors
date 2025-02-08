using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Category;
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

    public List<ProductImage> Images { get; private set; } = new List<ProductImage>();

    public static Product Create(
        string name,
        string description,
        string vendorCode,
        Guid categoryId,
        Guid carModelId,
        decimal price,
        decimal discount,
        int stock,
        bool isAvailable)
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

        RaiseDomainEvent(new ProductUpdatedEvent(Id));
    }


    public void UpdatePrice(decimal price)
    {
        Price = price;
    }

    public void UpdateDiscount(decimal discount)
    {
        Discount = discount;
    }

    public Result UpdateStock(int quantity)
    {
        if (Stock < quantity)
        {
            return Result.Failure(ProductErrors.NotEnoughStock(Stock)); 
        }

        Stock -= quantity;

        IsAvailable = Stock > 0;

        RaiseDomainEvent(new ProductStockUpdatedEvent(Id, Stock));

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

    public decimal CalculateDiscountedPrice()
    {
        if (Discount > 0)
        {
            return Price - Price * (Discount / 100);
        }
        return Price;
    }
}
