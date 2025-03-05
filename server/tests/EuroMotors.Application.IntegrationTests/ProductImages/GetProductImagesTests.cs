using Bogus;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.ProductImages.CreateProductImage;
using EuroMotors.Application.ProductImages.GetProductImageById;
using EuroMotors.Application.ProductImages.GetProductImagesByProductId;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.ProductImages;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.ProductImages;

public class GetProductImagesTests : BaseIntegrationTest
{
    public GetProductImagesTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenProductDoesNotExist()
    {
        // Arrange
        var query = new GetProductImagesByProductIdQuery(Guid.NewGuid());

        // Act
        Result<IReadOnlyCollection<ProductImageResponse>> result = await Sender.Send(query);

        // Assert
        result.Error.ShouldBe(ProductImageErrors.ProductImagesNotFound(query.ProductId));
    }

    [Fact]
    public async Task Should_ReturnProductImages_WhenProductExists()
    {
        // Arrange
        await CleanDatabaseAsync();

        var faker = new Faker();
        Guid categoryId = await Sender.CreateCategoryAsync(faker.Commerce.Categories(1)[0]);
        Guid carModelId = await Sender.CreateCarModelAsync(faker.Vehicle.Manufacturer(), faker.Vehicle.Model());

        Guid productId = await Sender.CreateProductAsync(
            "Product Name",
            "Product Description",
            "VendorCode123",
            categoryId,
            carModelId,
            100m,
            10m,
            10
        );

        await Sender.Send(new CreateProductImageCommand(new Uri("https://example.com/image.jpg"), productId)).ContinueWith(t => t.Result.Value, TaskScheduler.Default);

        var query = new GetProductImagesByProductIdQuery(productId);

        // Act
        Result<IReadOnlyCollection<ProductImageResponse>> result = await Sender.Send(query);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }
}
