using Bogus;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Products.DeleteProduct;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Products;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Products;

public class DeleteProductTests : BaseIntegrationTest
{
    public DeleteProductTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var command = new DeleteProductCommand(productId);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.Error.ShouldBe(ProductErrors.NotFound(productId));
    }

    [Fact]
    public async Task Should_ReturnSuccess_WhenProductIsDeleted()
    {
        var faker = new Faker();
        Guid categoryId = await Sender.CreateCategoryAsync(faker.Commerce.Categories(1)[0]);
        Guid carModelId = await Sender.CreateCarModelAsync(
            faker.Vehicle.Manufacturer(),
            faker.Vehicle.Model(),
            2020,
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel),
            null
        );

        // Arrange
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

        var command = new DeleteProductCommand(productId);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

}
