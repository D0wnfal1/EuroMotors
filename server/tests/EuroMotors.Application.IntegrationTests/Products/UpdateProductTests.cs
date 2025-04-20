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
    public UpdateProductTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenProductDoesNotExist()
    {
        // Arrange
        var nonExistingProductId = Guid.NewGuid();
        var faker = new Faker();
        var command = new UpdateProductCommand(
            nonExistingProductId,
            faker.Commerce.ProductName(),
            [],
            faker.Commerce.Ean13(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            faker.Random.Decimal(100, 1000),
            faker.Random.Decimal(0, 100),
            faker.Random.Int(1, 100));

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.Error.ShouldBe(ProductErrors.NotFound(nonExistingProductId));
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        var faker = new Faker();
        Guid categoryId = await Sender.CreateCategoryAsync(faker.Commerce.Categories(1)[0]);
        Guid carModelId = await Sender.CreateCarModelAsync(
            faker.Vehicle.Manufacturer(),
            faker.Vehicle.Model(),
            faker.Date.Past(10).Year,
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel),
            null
        );

        var specifications = new List<Specification>
        {
            new Specification("Color", "Red"),
            new Specification("Engine", "V8")
        };

        Guid productId = await Sender.CreateProductAsync(
            faker.Commerce.ProductName(),
            faker.Commerce.Ean13(),
            categoryId,
            carModelId,
            faker.Random.Decimal(100, 1000),
            faker.Random.Decimal(0, 100),
            10,
            specifications
        );

        var nonExistingCategoryId = Guid.NewGuid();
        var command = new UpdateProductCommand(
            productId,
            faker.Commerce.ProductName(),
            specifications,
            faker.Commerce.Ean13(),
            nonExistingCategoryId,
            carModelId,
            faker.Random.Decimal(100, 1000),
            faker.Random.Decimal(0, 100),
            faker.Random.Int(1, 100)
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
        var faker = new Faker();
        Guid categoryId = await Sender.CreateCategoryAsync(faker.Commerce.Categories(1)[0]);
        Guid carModelId = await Sender.CreateCarModelAsync(
            faker.Vehicle.Manufacturer(),
            faker.Vehicle.Model(),
            faker.Date.Past(10).Year,
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel),
            null
        );

        var specifications = new List<Specification>
        {
            new Specification("Color", "Red"),
            new Specification("Engine", "V8")
        };

        Guid productId = await Sender.CreateProductAsync(
            faker.Commerce.ProductName(),
            faker.Commerce.Ean13(),
            categoryId,
            carModelId,
            faker.Random.Decimal(100, 1000),
            faker.Random.Decimal(0, 100),
            10,
            specifications
        );

        var nonExistingCarModelId = Guid.NewGuid();
        var command = new UpdateProductCommand(
            productId,
            faker.Commerce.ProductName(),
            specifications,
            faker.Commerce.Ean13(),
            categoryId,
            nonExistingCarModelId,
            faker.Random.Decimal(100, 1000),
            faker.Random.Decimal(0, 100),
            faker.Random.Int(1, 100)
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
        var faker = new Faker();
        Guid categoryId = await Sender.CreateCategoryAsync(faker.Commerce.Categories(1)[0]);
        Guid carModelId = await Sender.CreateCarModelAsync(
            faker.Vehicle.Manufacturer(),
            faker.Vehicle.Model(),
            faker.Date.Past(10).Year,
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel),
            null
        );

        var specifications = new List<Specification>
        {
            new Specification("Color", "Red"),
            new Specification("Engine", "V8")
        };

        Guid productId = await Sender.CreateProductAsync(
            faker.Commerce.ProductName(),
            faker.Commerce.Ean13(),
            categoryId,
            carModelId,
            faker.Random.Decimal(100, 1000),
            faker.Random.Decimal(0, 100),
            10,
            specifications
        );

        string newName = faker.Commerce.ProductName();
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
            faker.Commerce.Ean13(),
            categoryId,
            carModelId,
            faker.Random.Decimal(100, 1000),
            faker.Random.Decimal(0, 100),
            faker.Random.Int(1, 100)
          );

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }
} 
