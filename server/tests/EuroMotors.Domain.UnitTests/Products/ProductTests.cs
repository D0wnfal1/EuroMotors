using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Products;
using EuroMotors.Domain.Products.Events;
using EuroMotors.Domain.UnitTests.Infrastructure;
using Shouldly;

namespace EuroMotors.Domain.UnitTests.Products;

public class ProductTests : BaseTest
{
    [Fact]
    public void Create_ShouldRaiseProductCreatedEvent()
    {
        // Act
        var product = Product.Create(ProductData.Name, ProductData.Description, ProductData.VendorCode, ProductData.CarModelId, ProductData.CarModelId, ProductData.Price,
            ProductData.Discount, ProductData.Stock, ProductData.IsAvailable);

        // Assert
        ProductCreatedDomainEvent @event = AssertDomainEventWasPublished<ProductCreatedDomainEvent>(product);
        @event.ProductId.ShouldBe(product.Id);
    }

    [Fact]
    public void Update_ShouldRaiseProductUpdatedEvent()
    {
        // Arrange
        var product = Product.Create(ProductData.Name, ProductData.Description, ProductData.VendorCode, ProductData.CarModelId, ProductData.CarModelId, ProductData.Price,
            ProductData.Discount, ProductData.Stock, ProductData.IsAvailable);

        // Act
        product.Update("Updated Name", "Updated Description", 150m, 5m, 100);

        // Assert
        ProductUpdatedDomainEvent @event = AssertDomainEventWasPublished<ProductUpdatedDomainEvent>(product);
        @event.ProductId.ShouldBe(product.Id);
    }

    [Fact]
    public void AddProductQuantity_ShouldRaiseProductStockUpdatedEvent()
    {
        // Arrange
        var product = Product.Create(ProductData.Name, ProductData.Description, ProductData.VendorCode, ProductData.CarModelId, ProductData.CarModelId, ProductData.Price,
            ProductData.Discount, ProductData.Stock, ProductData.IsAvailable);

        // Act
        product.AddProductQuantity(20);

        // Assert
        ProductStockUpdatedDomainEvent @event = AssertDomainEventWasPublished<ProductStockUpdatedDomainEvent>(product);
        @event.Stock.ShouldBe(70);
    }
    [Fact]
    public void MarkAsNotAvailable_ShouldRaiseProductIsNotAvailableDomainEvent()
    {
        // Arrange
        var product = Product.Create(ProductData.Name, ProductData.Description, ProductData.VendorCode, ProductData.CarModelId, ProductData.CarModelId, ProductData.Price,
            ProductData.Discount, ProductData.Stock, ProductData.IsAvailable);

        // Act
        product.MarkAsNotAvailable();

        // Assert
        Assert.False(product.IsAvailable);
        Assert.IsType<ProductIsNotAvailableDomainEvent>(product.DomainEvents[^1]);
    }

    [Fact]
    public void AddProductQuantity_ShouldUpdateStockCorrectly()
    {
        // Arrange
        var product = Product.Create(ProductData.Name, ProductData.Description, ProductData.VendorCode, ProductData.CarModelId, ProductData.CarModelId, ProductData.Price,
            ProductData.Discount, ProductData.Stock, ProductData.IsAvailable);

        // Act
        product.AddProductQuantity(20);

        // Assert
        Assert.Equal(70, product.Stock);
    }

    [Fact]
    public void SubtractProductQuantity_ShouldRaiseProductStockUpdatedEvent()
    {
        // Arrange
        var product = Product.Create(ProductData.Name, ProductData.Description, ProductData.VendorCode, ProductData.CarModelId, ProductData.CarModelId, ProductData.Price,
            ProductData.Discount, ProductData.Stock, ProductData.IsAvailable);

        // Act
        Result result = product.SubtractProductQuantity(10);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        ProductStockUpdatedDomainEvent @event = AssertDomainEventWasPublished<ProductStockUpdatedDomainEvent>(product);
        @event.Stock.ShouldBe(40);
    }

    [Fact]
    public void SubtractProductQuantity_ShouldFail_WhenNotEnoughStock()
    {
        // Arrange
        var product = Product.Create(ProductData.Name, ProductData.Description, ProductData.VendorCode, ProductData.CarModelId, ProductData.CarModelId, ProductData.Price,
            ProductData.Discount, 5, ProductData.IsAvailable);

        // Act
        Result result = product.SubtractProductQuantity(10);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(ProductErrors.NotEnoughStock(5));
    }

    [Fact]
    public void UpdateStock_ShouldSetCorrectStock()
    {
        // Arrange
        var product = Product.Create(ProductData.Name, ProductData.Description, ProductData.VendorCode, ProductData.CarModelId, ProductData.CarModelId, ProductData.Price,
            ProductData.Discount, ProductData.Stock, ProductData.IsAvailable);

        // Act
        product.UpdateStock(50);

        // Assert
        product.Stock.ShouldBe(50);
    }
}
