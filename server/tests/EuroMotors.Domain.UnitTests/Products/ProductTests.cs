using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;
using EuroMotors.Domain.Products.Events;
using EuroMotors.Domain.UnitTests.Infrastructure;
using Shouldly;

namespace EuroMotors.Domain.UnitTests.Products;

public class ProductTests : BaseTest
{
    private static readonly Guid CategoryId = Guid.NewGuid();
    private static readonly Guid CarModelId = Guid.NewGuid();
    private const string Name = "Test Product";
    private const string VendorCode = "TP123";
    private const decimal Price = 100.00m;
    private const decimal Discount = 10.00m;
    private const int Stock = 10;
    private static readonly IEnumerable<(string Name, string Value)> Specifications = new[]
    {
        ("Color", "Red"),
        ("Size", "Medium")
    };

    [Fact]
    public void Create_Should_SetPropertyValues()
    {
        // Act
        var product = Product.Create(
            Name,
            Specifications,
            VendorCode,
            CategoryId,
            CarModelId,
            Price,
            Discount,
            Stock);

        // Assert
        product.Id.ShouldNotBe(Guid.Empty);
        product.Name.ShouldBe(Name);
        product.VendorCode.ShouldBe(VendorCode);
        product.CategoryId.ShouldBe(CategoryId);
        product.CarModelId.ShouldBe(CarModelId);
        product.Price.ShouldBe(Price);
        product.Discount.ShouldBe(Discount);
        product.Stock.ShouldBe(Stock);
        product.IsAvailable.ShouldBeTrue();
        product.Slug.Value.ShouldNotBeNullOrEmpty();
        product.Specifications.Count.ShouldBe(2);
    }

    [Fact]
    public void Create_Should_RaiseProductCreatedDomainEvent()
    {
        // Act
        var product = Product.Create(
            Name,
            Specifications,
            VendorCode,
            CategoryId,
            CarModelId,
            Price,
            Discount,
            Stock);

        // Assert
        ProductCreatedDomainEvent domainEvent = AssertDomainEventWasPublished<ProductCreatedDomainEvent>(product);
        domainEvent.ProductId.ShouldBe(product.Id);
    }

    [Fact]
    public void Update_Should_UpdatePropertyValues()
    {
        // Arrange
        var product = Product.Create(
            Name,
            Specifications,
            VendorCode,
            CategoryId,
            CarModelId,
            Price,
            Discount,
            Stock);

        string newName = "Updated Product";
        string newVendorCode = "UP456";
        var newCategoryId = Guid.NewGuid();
        var newCarModelId = Guid.NewGuid();
        decimal newPrice = 150.00m;
        decimal newDiscount = 15.00m;
        int newStock = 5;
        (string, string)[] newSpecifications =
        [
            ("Color", "Blue"),
            ("Size", "Large"),
            ("Weight", "2kg")
        ];

        // Act
        product.Update(
            newName,
            newSpecifications,
            newVendorCode,
            newCategoryId,
            newCarModelId,
            newPrice,
            newDiscount,
            newStock);

        // Assert
        product.Name.ShouldBe(newName);
        product.VendorCode.ShouldBe(newVendorCode);
        product.CategoryId.ShouldBe(newCategoryId);
        product.CarModelId.ShouldBe(newCarModelId);
        product.Price.ShouldBe(newPrice);
        product.Discount.ShouldBe(newDiscount);
        product.Stock.ShouldBe(newStock);
        product.IsAvailable.ShouldBeTrue();
        product.Specifications.Count.ShouldBe(3);
    }

    [Fact]
    public void Update_Should_RaiseProductUpdatedDomainEvent()
    {
        // Arrange
        var product = Product.Create(
            Name,
            Specifications,
            VendorCode,
            CategoryId,
            CarModelId,
            Price,
            Discount,
            Stock);

        // Act
        product.Update(
            "Updated Product",
            Specifications,
            "UP456",
            CategoryId,
            CarModelId,
            Price,
            Discount,
            Stock);

        // Assert
        ProductUpdatedDomainEvent domainEvent = AssertDomainEventWasPublished<ProductUpdatedDomainEvent>(product);
        domainEvent.ProductId.ShouldBe(product.Id);
    }

    [Fact]
    public void SubtractProductQuantity_Should_DecreaseStock()
    {
        // Arrange
        var product = Product.Create(
            Name,
            Specifications,
            VendorCode,
            CategoryId,
            CarModelId,
            Price,
            Discount,
            Stock);

        // Act
        Result result = product.SubtractProductQuantity(3);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        product.Stock.ShouldBe(7);
        product.IsAvailable.ShouldBeTrue();
        ProductStockUpdatedDomainEvent domainEvent = AssertDomainEventWasPublished<ProductStockUpdatedDomainEvent>(product);
        domainEvent.ProductId.ShouldBe(product.Id);
        domainEvent.Stock.ShouldBe(7);
    }

    [Fact]
    public void SubtractProductQuantity_Should_ReturnFailure_WhenNotEnoughStock()
    {
        // Arrange
        var product = Product.Create(
            Name,
            Specifications,
            VendorCode,
            CategoryId,
            CarModelId,
            Price,
            Discount,
            Stock);

        // Act
        Result result = product.SubtractProductQuantity(15);

        // Assert
        result.IsFailure.ShouldBeTrue();
        product.Stock.ShouldBe(10);
    }

    [Fact]
    public void AddProductQuantity_Should_IncreaseStock()
    {
        // Arrange
        var product = Product.Create(
            Name,
            Specifications,
            VendorCode,
            CategoryId,
            CarModelId,
            Price,
            Discount,
            0);

        // Act
        Result result = product.AddProductQuantity(5);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        product.Stock.ShouldBe(5);
        product.IsAvailable.ShouldBeTrue();
        ProductStockUpdatedDomainEvent domainEvent = AssertDomainEventWasPublished<ProductStockUpdatedDomainEvent>(product);
        domainEvent.ProductId.ShouldBe(product.Id);
        domainEvent.Stock.ShouldBe(5);
    }

    [Fact]
    public void SetAvailability_Should_UpdateIsAvailable_AndRaiseEvent_WhenChangingToAvailable()
    {
        // Arrange
        var product = Product.Create(
            Name,
            Specifications,
            VendorCode,
            CategoryId,
            CarModelId,
            Price,
            Discount,
            0);
        
        // Act
        product.SetAvailability(true);

        // Assert
        product.IsAvailable.ShouldBeTrue();
        ProductIsAvailableDomainEvent domainEvent = AssertDomainEventWasPublished<ProductIsAvailableDomainEvent>(product);
        domainEvent.ProductId.ShouldBe(product.Id);
    }

    [Fact]
    public void SetAvailability_Should_UpdateIsAvailable_AndRaiseEvent_WhenChangingToUnavailable()
    {
        // Arrange
        var product = Product.Create(
            Name,
            Specifications,
            VendorCode,
            CategoryId,
            CarModelId,
            Price,
            Discount,
            Stock);
        
        // Act
        product.SetAvailability(false);

        // Assert
        product.IsAvailable.ShouldBeFalse();
        ProductIsNotAvailableDomainEvent domainEvent = AssertDomainEventWasPublished<ProductIsNotAvailableDomainEvent>(product);
        domainEvent.ProductId.ShouldBe(product.Id);
    }

    [Fact]
    public void SetAvailability_Should_NotRaiseEvent_WhenAvailabilityUnchanged()
    {
        // Arrange
        var product = Product.Create(
            Name,
            Specifications,
            VendorCode,
            CategoryId,
            CarModelId,
            Price,
            Discount,
            Stock);
        
        int initialDomainEventsCount = product.DomainEvents.Count;
        
        // Act
        product.SetAvailability(true);

        // Assert
        product.IsAvailable.ShouldBeTrue();
        product.DomainEvents.Count.ShouldBe(initialDomainEventsCount);
    }
}
