using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.ProductImages;
using EuroMotors.Domain.ProductImages.Events;
using EuroMotors.Domain.UnitTests.Infrastructure;
using Shouldly;

namespace EuroMotors.Domain.UnitTests.ProductImages;

public class ProductImageTests : BaseTest
{
    [Fact]
    public void Create_ShouldRaiseProductImageCreatedEvent()
    {
        // Act
        var productImage = ProductImage.Create(ProductImageData.Url, ProductImageData.ProductId);

        // Assert
        ProductImageCreatedDomainEvent @event = AssertDomainEventWasPublished<ProductImageCreatedDomainEvent>(productImage);
        @event.ProductImageId.ShouldBe(productImage.Id);
    }

    [Fact]
    public void Create_ShouldSetPropertiesCorrectly()
    {
        // Act
        var productImage = ProductImage.Create(new string("https://example.com/image.png"), Guid.NewGuid());

        // Assert
        productImage.Url.ShouldBe(new string("https://example.com/image.png"));
        productImage.ProductId.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void UpdateUrl_ShouldRaiseProductImageUpdatedEvent()
    {
        // Arrange
        var productImage = ProductImage.Create(ProductImageData.Url, ProductImageData.ProductId);

        // Act
        productImage.UpdateImage(new string("https://example.com/updated.png"), ProductImageData.ProductId);

        // Assert
        ProductImageUpdatedDomainEvent @event = AssertDomainEventWasPublished<ProductImageUpdatedDomainEvent>(productImage);
        @event.ProductImageId.ShouldBe(productImage.Id);
    }

    [Fact]
    public void Delete_ShouldRaiseProductImageDeletedDomainEvent()
    {
        // Arrange
        var productImage = ProductImage.Create(ProductImageData.Url, ProductImageData.ProductId);

        // Act
        Result result = productImage.Delete();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        ProductImageDeletedDomainEvent @event = AssertDomainEventWasPublished<ProductImageDeletedDomainEvent>(productImage);
        @event.ProductImageId.ShouldBe(productImage.Id);
    }

    [Fact]
    public void Delete_ShouldFail_WhenProductImageNotFound()
    {
        // Arrange
        var productImage = ProductImage.Create(ProductImageData.Url, Guid.Empty);

        // Act
        Result result = productImage.Delete();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("ProductImage.ProductImageNotFound");
    }
}
