using Bogus;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Products.GetProductById;
using EuroMotors.Application.Products.GetProducts;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Products;
using Microsoft.Extensions.DependencyInjection;
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

        Guid categoryId = await ServiceProvider.CreateCategoryAsync(_faker.Commerce.Categories(1)[0]);
        Guid brandId = await ServiceProvider.CreateCarBrandAsync(_faker.Vehicle.Manufacturer());
        Guid carModelId = await ServiceProvider.CreateCarModelAsync(
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
        await ServiceProvider.CreateProductAsync(
            "Product1",
            _faker.Commerce.ProductName(),
            categoryId,
            carModelId,
            100m,
            10m,
            5,
            specifications
        );

        await ServiceProvider.CreateProductAsync(
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
        IQueryHandler<GetProductsQuery, Pagination<ProductResponse>> handler = ServiceProvider.GetRequiredService<IQueryHandler<GetProductsQuery, Pagination<ProductResponse>>>();
        Result<Pagination<ProductResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Value.Count.ShouldBe(2);
    }
}
