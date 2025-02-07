using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Category;
using EuroMotors.Domain.Products.Events;

namespace EuroMotors.Domain.Products;

public sealed class Product : Entity
{
    private Product(Guid id,
        Name name,
        Description description,
        VendorCode vendorCode,
        Guid categoryId,
        Guid carModelId,
        decimal price,
        decimal? discount,
        int stock,
        bool isAvailable)
        : base(id)
    {
        Name = name;
        Description = description;
        VendorCode = vendorCode;
        CategoryId = categoryId;
        CarModelId = carModelId;
        Price = price;
        Discount = discount;
        Stock = stock;
        IsAvailable = isAvailable;
    }

    public Guid CategoryId { get; private set; }

    public Guid CarModelId { get; private set; }

    public Name Name { get; private set; }

    public Description Description { get; private set; }

    public VendorCode VendorCode { get; private set; }

    public decimal Price { get; private set; }

    public decimal? Discount { get; private set; }

    public int Stock { get; private set; }

    public bool IsAvailable { get; private set; }

    // public string ImageUrl { get; private set; }

    public static Product Create(
        Name name,
        Description description,
        VendorCode vendorCode,
        Guid categoryId,
        Guid carModelId,
        decimal price,
        decimal? discount,
        int stock,
        bool isAvailable)
    {
        Product product = new(
            Guid.NewGuid(),
            name,
            description,
            vendorCode,
            categoryId,
            carModelId,
            price,
            discount,
            stock,
            isAvailable);


        product.RaiseDomainEvents(new ProductCreatedDomainEvent(product.Id));

        return product;
    }

    public void Archive()
    {
        if (IsAvailable)
        {
            return;
        }

        IsAvailable = true;

        RaiseDomainEvents(new ProductIsNotAvailableDomainEvent(Id));
    }
}
