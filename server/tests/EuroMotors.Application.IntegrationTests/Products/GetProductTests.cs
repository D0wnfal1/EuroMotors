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
    private readonly Faker _faker = new();

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

        Guid categoryId = await Sender.CreateCategoryAsync(_faker.Commerce.Categories(1)[0]);
        Guid brandId = await Sender.CreateCarBrandAsync(_faker.Vehicle.Manufacturer());
        Guid carModelId = await Sender.CreateCarModelAsync(
            brandId,
            _faker.Vehicle.Model(),
            2020,
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel)
        );

        var specifications = new List<Specification>
        {
            new Specification ("Color", "Red" ),
            new Specification ("Engine", "V8")
        };

        Guid productId = await Sender.CreateProductAsync(
            "Product Name",
            "VendorCode123",
            categoryId,
            carModelId,
            100m,
            10m,
            10,
            specifications

        );

        var query = new GetProductByIdQuery(productId);

        // Act
        Result<ProductResponse> result = await Sender.Send(query);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }
}
