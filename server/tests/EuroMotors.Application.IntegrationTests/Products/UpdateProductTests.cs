using Bogus;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Products.UpdateProduct;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Products;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Products;

public class UpdateProductTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public UpdateProductTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenProductDoesNotExist()
    {
        // Arrange
        var nonExistingProductId = Guid.NewGuid();
        var command = new UpdateProductCommand(
            nonExistingProductId,
            _faker.Commerce.ProductName(),
            [],
            _faker.Commerce.Ean13(),
            Guid.NewGuid(),
            _faker.Random.Decimal(100, 1000),
            _faker.Random.Decimal(0, 100),
            _faker.Random.Int(1, 100));

        // Act
        ICommandHandler<UpdateProductCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<UpdateProductCommand>>();
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Error.ShouldBe(ProductErrors.NotFound(nonExistingProductId));
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        Guid categoryId = await ServiceProvider.CreateCategoryAsync("Test Category11111");
        Guid brandId = await ServiceProvider.CreateCarBrandAsync("Test Brand11111");
        Guid carModelId = await ServiceProvider.CreateCarModelAsync(
            brandId,
            _faker.Vehicle.Model(),
            _faker.Random.Int(2000, 2023),
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel)
        );

        var specifications = new List<Specification>
        {
            new Specification("Color", "Red"),
            new Specification("Engine", "V8")
        };

        Guid productId = await ServiceProvider.CreateProductAsync(
            _faker.Commerce.ProductName(),
            _faker.Commerce.Ean13(),
            categoryId,
            carModelId,
            _faker.Random.Decimal(100, 1000),
            _faker.Random.Decimal(0, 100),
            10,
            specifications
        );

        var nonExistCategory = Guid.NewGuid();

        var command = new UpdateProductCommand(
            productId,
            _faker.Commerce.ProductName(),
            specifications,
            _faker.Commerce.Ean13(),
            nonExistCategory,
            _faker.Random.Decimal(100, 1000),
            _faker.Random.Decimal(0, 100),
            _faker.Random.Int(1, 100)
        );

        // Act
        ICommandHandler<UpdateProductCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<UpdateProductCommand>>();
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Error.ShouldBe(CategoryErrors.NotFound(nonExistCategory));
    }

    [Fact]
    public async Task Should_ReturnSuccess_WhenUpdateIsValid()
    {
        // Arrange
        Guid categoryId = await ServiceProvider.CreateCategoryAsync("Test Category11");
        Guid brandId = await ServiceProvider.CreateCarBrandAsync("Test Brand1111");
        Guid carModelId = await ServiceProvider.CreateCarModelAsync(
            brandId,
            "TestModel",
            _faker.Random.Int(2000, 2023),
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel)
        );

        var specifications = new List<Specification>
        {
            new Specification("Color", "Red"),
            new Specification("Engine", "V8")
        };

        Guid productId = await ServiceProvider.CreateProductAsync(
            _faker.Commerce.ProductName(),
            _faker.Commerce.Ean13(),
            categoryId,
            carModelId,
            _faker.Random.Decimal(100, 1000),
            _faker.Random.Decimal(0, 100),
            10,
            specifications
        );

        string newName = _faker.Commerce.ProductName();
        var updatedSpecifications = new List<Specification>
        {
            new Specification("Color", "Blue"),
            new Specification("Engine", "V6"),
            new Specification("Material", "Aluminum")
        };

        var command = new UpdateProductCommand(
            productId,
            newName,
            updatedSpecifications,
            _faker.Commerce.Ean13(),
            categoryId,
            _faker.Random.Decimal(100, 1000),
            _faker.Random.Decimal(0, 100),
            _faker.Random.Int(1, 100)
          );

        // Act
        ICommandHandler<UpdateProductCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<UpdateProductCommand>>();
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }
}
