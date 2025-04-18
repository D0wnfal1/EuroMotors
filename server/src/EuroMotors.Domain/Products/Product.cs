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

    private readonly List<Specification> _specifications = [];
    public IReadOnlyCollection<Specification> Specifications => _specifications.AsReadOnly();

    public string VendorCode { get; private set; }

    public decimal Price { get; private set; }

    public decimal Discount { get; private set; } = 0m;

    public int Stock { get; private set; }

    public bool IsAvailable { get; private set; }

    public Slug Slug { get; private set; }

    public List<ProductImage> Images { get; private set; } = [];

    public static Product Create(
        string name,
        IEnumerable<(string Name, string Value)>? specifications,
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
            VendorCode = vendorCode,
            CategoryId = categoryId,
            CarModelId = carModelId,
            Price = price,
            Discount = discount,
            Stock = stock,
            IsAvailable = stock > 0,
            Slug = Slug.GenerateSlug(name)
        };
        if (specifications is not null)
        {
            foreach ((string specName, string specValue) in specifications)
            {
                product.AddSpecification(specName, specValue);
            }
        }

        product.RaiseDomainEvent(new ProductCreatedDomainEvent(product.Id));

        return product;
    }

    public void Update(
        string name,
        IEnumerable<(string Name, string Value)>? specifications,
        string vendorCode,
        Guid categoryId,
        Guid carModelId,
        decimal price,
        decimal discount,
        int stock
       )
    {
        Name = name;
        VendorCode = vendorCode;
        CategoryId = categoryId;
        CarModelId = carModelId;
        Price = price;
        Discount = discount;
        Stock = stock;
        IsAvailable = stock > 0;
        Slug = Slug.GenerateSlug(name);

        if (specifications is not null)
        {
            UpdateSpecifications(specifications);
        }

        RaiseDomainEvent(new ProductUpdatedDomainEvent(Id));
    }

    public void AddSpecification(string name, string value)
    {
        var spec = new Specification(name, value);
        _specifications.Add(spec);
    }

    public void UpdateSpecifications(IEnumerable<(string Name, string Value)> specifications)
    {
        _specifications.Clear();

        foreach ((string name, string value) in specifications)
        {
            _specifications.Add(new Specification(name, value));
        }
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

        IsAvailable = Stock > 0;

        RaiseDomainEvent(new ProductStockUpdatedDomainEvent(Id, Stock));

        return Result.Success();
    }

    public void SetAvailability(bool isAvailable)
    {
        if (IsAvailable == isAvailable)
        {
            return;
        }

        IsAvailable = isAvailable;

        if (isAvailable)
        {
            RaiseDomainEvent(new ProductIsAvailableDomainEvent(Id));
        }
        else
        {
            RaiseDomainEvent(new ProductIsNotAvailableDomainEvent(Id));
        }
    }
}
