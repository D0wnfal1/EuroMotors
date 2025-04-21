using Bogus;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Products.UpdateProduct;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Products;
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
            Guid.NewGuid(),
            _faker.Random.Decimal(100, 1000),
            _faker.Random.Decimal(0, 100),
            _faker.Random.Int(1, 100));

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.Error.ShouldBe(ProductErrors.NotFound(nonExistingProductId));
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        Guid categoryId = await Sender.CreateCategoryAsync("Test Category1");
        Guid brandId = await Sender.CreateCarBrandAsync("Test Brand11");
        Guid carModelId = await Sender.CreateCarModelAsync(
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

        Guid productId = await Sender.CreateProductAsync(
            _faker.Commerce.ProductName(),
            _faker.Commerce.Ean13(),
            categoryId,
            carModelId,
            _faker.Random.Decimal(100, 1000),
            _faker.Random.Decimal(0, 100),
            10,
            specifications
        );

        var nonExistingCategoryId = Guid.NewGuid();
        var command = new UpdateProductCommand(
            productId,
            _faker.Commerce.ProductName(),
            specifications,
            _faker.Commerce.Ean13(),
            nonExistingCategoryId,
            carModelId,
            _faker.Random.Decimal(100, 1000),
            _faker.Random.Decimal(0, 100),
            _faker.Random.Int(1, 100)
            );

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCarModelDoesNotExist()
    {
        // Arrange
        Guid categoryId = await Sender.CreateCategoryAsync(_faker.Commerce.Categories(1)[0]);
        Guid brandId = await Sender.CreateCarBrandAsync(_faker.Vehicle.Manufacturer());
        Guid carModelId = await Sender.CreateCarModelAsync(
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

        Guid productId = await Sender.CreateProductAsync(
            _faker.Commerce.ProductName(),
            _faker.Commerce.Ean13(),
            categoryId,
            carModelId,
            _faker.Random.Decimal(100, 1000),
            _faker.Random.Decimal(0, 100),
            10,
            specifications
        );

        var nonExistingCarModelId = Guid.NewGuid();
        var command = new UpdateProductCommand(
            productId,
            _faker.Commerce.ProductName(),
            specifications,
            _faker.Commerce.Ean13(),
            categoryId,
            nonExistingCarModelId,
            _faker.Random.Decimal(100, 1000),
            _faker.Random.Decimal(0, 100),
            _faker.Random.Int(1, 100)
         );

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public async Task Should_ReturnSuccess_WhenUpdateIsValid()
    {
        // Arrange
        Guid categoryId = await Sender.CreateCategoryAsync("Test Category11");
        Guid brandId = await Sender.CreateCarBrandAsync("Test Brand1111");
        Guid carModelId = await Sender.CreateCarModelAsync(
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

        Guid productId = await Sender.CreateProductAsync(
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
            carModelId,
            _faker.Random.Decimal(100, 1000),
            _faker.Random.Decimal(0, 100),
            _faker.Random.Int(1, 100)
          );

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }
}
