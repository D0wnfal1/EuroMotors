using Bogus;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Products.GetProductById;
using EuroMotors.Application.Products.GetProducts;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Products;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Products;

public class GetProductsTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public GetProductsTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnProducts_WhenProductsExist()
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
            new Specification ( "Color", "Red" ),
            new Specification ("Engine", "V8")
        };
        await Sender.CreateProductAsync(
            "Product1",
            _faker.Commerce.ProductName(),
            categoryId,
            carModelId,
            100m,
            10m,
            5,
            specifications
        );

        await Sender.CreateProductAsync(
            "Product2",
            _faker.Commerce.ProductName(),
            categoryId,
            carModelId,
            150m,
            15m,
            10,
            specifications
        );


        var query = new GetProductsQuery(null, null, null, null, false, false, false, 1, 10);

        // Act
        Result<Pagination<ProductResponse>> result = await Sender.Send(query);

        // Assert
        result.Value.Count.ShouldBe(2);
    }
}
