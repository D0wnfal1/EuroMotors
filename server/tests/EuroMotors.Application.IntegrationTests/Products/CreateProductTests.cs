using Bogus;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Products.CreateProduct;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Categories;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Products;

public class CreateProductTests : BaseIntegrationTest
{
    public CreateProductTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenPriceIsInvalid()
    {
        // Arrange
        var faker = new Faker();
        Guid categoryId = await Sender.CreateCategoryAsync(faker.Commerce.Categories(1)[0]);
        Guid carModelId = await Sender.CreateCarModelAsync(faker.Vehicle.Manufacturer(), faker.Vehicle.Model());

        var command = new CreateProductCommand(
            faker.Commerce.ProductName(),
            faker.Commerce.ProductDescription(),
            faker.Commerce.Ean13(),
            categoryId,
            carModelId,
            -100,
            faker.Random.Decimal(0, 100),
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
        var faker = new Faker();
        var categoryId = Guid.NewGuid();
        Guid carModelId = await Sender.CreateCarModelAsync(faker.Vehicle.Manufacturer(), faker.Vehicle.Model());

        var command = new CreateProductCommand(
            faker.Commerce.ProductName(),
            faker.Commerce.ProductDescription(),
            faker.Commerce.Ean13(),
            categoryId,
            carModelId,
            faker.Random.Decimal(100, 1000),
            faker.Random.Decimal(0, 100),
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
        var faker = new Faker();
        Guid categoryId = await Sender.CreateCategoryAsync(faker.Commerce.Categories(1)[0]);
        var carModelId = Guid.NewGuid();

        var command = new CreateProductCommand(
            faker.Commerce.ProductName(),
            faker.Commerce.ProductDescription(),
            faker.Commerce.Ean13(),
            categoryId,
            carModelId,
            faker.Random.Decimal(100, 1000),
            faker.Random.Decimal(0, 100),
            10);

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.Error.ShouldBe(CarModelErrors.NotFound(carModelId));
    }

    [Fact]
    public async Task Should_CreateProduct_WhenCommandIsValid()
    {
        // Arrange
        await CleanDatabaseAsync();
        var faker = new Faker();
        Guid categoryId = await Sender.CreateCategoryAsync(faker.Commerce.Categories(1)[0]);
        Guid carModelId = await Sender.CreateCarModelAsync(faker.Vehicle.Manufacturer(), faker.Vehicle.Model());

        var command = new CreateProductCommand(
            faker.Commerce.ProductName(),
            faker.Commerce.ProductDescription(),
            faker.Commerce.Ean13(),
            categoryId,
            carModelId,
            faker.Random.Decimal(100, 1000),
            faker.Random.Decimal(0, 100),
            10);

        // Act
        Result<Guid> result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(Guid.Empty);
    }
}
