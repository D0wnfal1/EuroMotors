using Bogus;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Products.GetProductById;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Products;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Products;

public class GetProductTests : BaseIntegrationTest
{
    public GetProductTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenProductDoesNotExist()
    {
        // Arrange
        var query = new GetProductByIdQuery(Guid.NewGuid());

        // Act
        Result<ProductResponse> result = await Sender.Send(query);

        // Assert
        result.Error.ShouldBe(ProductErrors.NotFound(query.ProductId));
    }

    [Fact]
    public async Task Should_ReturnProduct_WhenProductExists()
    {
        // Arrange
        await CleanDatabaseAsync();

        var faker = new Faker();
        Guid categoryId = await Sender.CreateCategoryAsync(faker.Commerce.Categories(1)[0]);
        Guid carModelId = await Sender.CreateCarModelAsync(
            faker.Vehicle.Manufacturer(),
            faker.Vehicle.Model(),
            2020, 
            null,
            BodyType.Sedan, 
            new EngineSpec (6, FuelType.Diesel, 6),
            null 
        );

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

        var query = new GetProductByIdQuery(productId);

        // Act
        Result<ProductResponse> result = await Sender.Send(query);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }
}
