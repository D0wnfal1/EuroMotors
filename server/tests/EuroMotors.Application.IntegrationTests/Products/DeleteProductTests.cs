using Bogus;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Products.DeleteProduct;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Products;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Products;

public class DeleteProductTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

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
        ICommandHandler<DeleteProductCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<DeleteProductCommand>>();
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Error.ShouldBe(ProductErrors.NotFound(productId));
    }

    [Fact]
    public async Task Should_ReturnSuccess_WhenProductIsDeleted()
    {
        Guid categoryId = await ServiceProvider.CreateCategoryAsync("Category Cateory9");
        Guid brandId = await ServiceProvider.CreateCarBrandAsync("Brand brand9");
        Guid carModelId = await ServiceProvider.CreateCarModelAsync(
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

        // Arrange
        Guid productId = await ServiceProvider.CreateProductAsync(
            "Product Name",
            "VendorCode123",
            categoryId,
            carModelId,
            100m,
            10m,
            10,
            specifications
        );

        var command = new DeleteProductCommand(productId);

        // Act
        ICommandHandler<DeleteProductCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<DeleteProductCommand>>();
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }
}
