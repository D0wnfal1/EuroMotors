using Bogus;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Products.GetProductById;
using EuroMotors.Application.Products.GetProducts;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Products;

public class GetProductsTests : BaseIntegrationTest
{
    public GetProductsTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenProductsDoNotExist()
    {
        // Arrange
        await CleanDatabaseAsync();

        var query = new GetProductsQuery(null, null, null, null, 1, 10);


        // Act
        Result<Pagination<ProductResponse>> result = await Sender.Send(query);

        // Assert
        result.Value.Data.ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_ReturnProducts_WhenProductsExist()
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
            new EngineSpec(6, FuelType.Diesel, 6),
            null
        );
        await Sender.CreateProductAsync(
            "Product1",
            "Description1",
            faker.Commerce.ProductName(),
            categoryId,
        carModelId,
            100m,
            10m,
            5
        );

        await Sender.CreateProductAsync(
            "Product2",
            "Description2",
            faker.Commerce.ProductName(),
            categoryId,
            carModelId,
            150m,
            15m,
            10
        );


        var query = new GetProductsQuery(null, null, null, null, 1, 10);

        // Act
        Result<Pagination<ProductResponse>> result = await Sender.Send(query);

        // Assert
        result.Value.Count.ShouldBe(2);
    }

}
