using Bogus;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.ProductImages.CreateProductImage;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.ProductImages;
using Org.BouncyCastle.Asn1.Ocsp;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.ProductImages;

public class CreateProductImageTests : BaseIntegrationTest
{
    public CreateProductImageTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_CreateProductImage_WhenCommandIsValid()
    {
        // Arrange
        await CleanDatabaseAsync();
        var faker = new Faker();
        Guid categoryId = await Sender.CreateCategoryAsync(faker.Commerce.Categories(1)[0]);
        Guid carModelId = await Sender.CreateCarModelAsync(faker.Vehicle.Manufacturer(), faker.Vehicle.Model());
        Guid productId = await Sender.CreateProductAsync("Product Name", "Product Description", "1234567890123",
            categoryId, carModelId, 100m, 10m, 5);

        var command = new CreateProductImageCommand(new Uri("https://example.com/image.jpg"), productId);

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(Guid.Empty);
    }

}
