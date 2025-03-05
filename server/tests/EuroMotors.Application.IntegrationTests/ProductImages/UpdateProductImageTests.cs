using Bogus;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.ProductImages.CreateProductImage;
using EuroMotors.Application.ProductImages.UpdateProductImage;
using EuroMotors.Domain.Abstractions;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.ProductImages;

public class UpdateProductImageTests : BaseIntegrationTest
{
    public UpdateProductImageTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_UpdateProductImage_WhenCommandIsValid()
    {
        // Arrange
        await CleanDatabaseAsync();
        var faker = new Faker();
        Guid categoryId = await Sender.CreateCategoryAsync(faker.Commerce.Categories(1)[0]);
        Guid carModelId = await Sender.CreateCarModelAsync(faker.Vehicle.Manufacturer(), faker.Vehicle.Model());
        Guid productId = await Sender.CreateProductAsync("Product Name", "Product Description", "1234567890123", categoryId, carModelId, 100m, 10m, 5);

        Guid imageId = await Sender.Send(new CreateProductImageCommand(new Uri("https://example.com/image.jpg"), productId)).ContinueWith(t => t.Result.Value, TaskScheduler.Default);
        var command = new UpdateProductImageCommand(imageId, new Uri("https://example.com/updated-image.jpg"), productId);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }
}
