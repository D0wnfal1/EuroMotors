using Bogus;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.ProductImages.CreateProductImage;
using EuroMotors.Application.ProductImages.GetProductImageById;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.ProductImages;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.ProductImages;

public class GetProductImageTests : BaseIntegrationTest
{
    public GetProductImageTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenProductDoesNotExist()
    {
        // Arrange
        var query = new GetProductImageByIdQuery(Guid.NewGuid());

        // Act
        Result<ProductImageResponse> result = await Sender.Send(query);

        // Assert
        result.Error.ShouldBe(ProductImageErrors.ProductImageNotFound(query.ProductImageId));
    }

    [Fact]
    public async Task Should_ReturnProduct_WhenProductExists()
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

        Guid productImageId = await Sender.Send(new CreateProductImageCommand(new Uri("https://example.com/image.jpg"), productId)).ContinueWith(t => t.Result.Value, TaskScheduler.Default);

        var query = new GetProductImageByIdQuery(productImageId);

        // Act
        Result<ProductImageResponse> result = await Sender.Send(query);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }
}
