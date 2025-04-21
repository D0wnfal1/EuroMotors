using Bogus;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Products.CreateProduct;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Products;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Products;

public class CreateProductTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public CreateProductTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenPriceIsInvalid()
    {
        // Arrange
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

        var command = new CreateProductCommand(
            _faker.Commerce.ProductName(),
            specifications,
            _faker.Commerce.Ean13(),
            categoryId,
            carModelId,
            -100,
            _faker.Random.Decimal(0, 100),
            10);

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.Error.ShouldBeOfType<ValidationError>();
        result.Error.Code.ShouldBe("Validation.General");
        result.Error.Description.ShouldContain("One or more validation errors occurred");
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCategoryDoesNotExist()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
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

        var command = new CreateProductCommand(
            _faker.Commerce.ProductName(),
            specifications,
            _faker.Commerce.Ean13(),
            categoryId,
            carModelId,
            _faker.Random.Decimal(100, 1000),
            _faker.Random.Decimal(0, 100),
            10);

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.Error.ShouldBe(CategoryErrors.NotFound(categoryId));
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenCarModelDoesNotExist()
    {
        // Arrange
        Guid categoryId = await Sender.CreateCategoryAsync(_faker.Commerce.Categories(1)[0]);
        var carModelId = Guid.NewGuid();

        var specifications = new List<Specification>
        {
            new Specification ("Color", "Red" ),
            new Specification ("Engine", "V8")
        };

        var command = new CreateProductCommand(
            _faker.Commerce.ProductName(),
            specifications,
            _faker.Commerce.Ean13(),
            categoryId,
            carModelId,
            _faker.Random.Decimal(100, 1000),
            _faker.Random.Decimal(0, 100),
            10);

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.Error.ShouldBe(CarModelErrors.ModelNotFound(carModelId));
    }

    [Fact]
    public async Task Should_CreateProduct_WhenCommandIsValid()
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

        var command = new CreateProductCommand(
            _faker.Commerce.ProductName(),
            specifications,
            _faker.Commerce.Ean13(),
            categoryId,
            carModelId,
            _faker.Random.Decimal(100, 1000),
            _faker.Random.Decimal(0, 100),
            10);

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(Guid.Empty);
    }
}
