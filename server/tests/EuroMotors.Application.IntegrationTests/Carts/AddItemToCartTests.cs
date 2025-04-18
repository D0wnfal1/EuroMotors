using Bogus;
using EuroMotors.Application.CarModels.CreateCarModel;
using EuroMotors.Application.Carts.AddItemToCart;
using EuroMotors.Application.Categories.CreateCategory;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Products.CreateProduct;
using EuroMotors.Application.Users.Register;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Products;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Carts;

public class AddItemToCartTests : BaseIntegrationTest
{
    private const int Quantity = 10;

    public AddItemToCartTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenUserDoesNotExist()
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
            new Specification ("Color", "Red"),
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

        var command = new AddItemToCartCommand(
            faker.Random.Guid(),
            productId,
            Quantity);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.Error.Type.ShouldBe(ErrorType.Failure);
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenProductDoesNotExist()
    {
        // Arrange
        Guid userId = await Sender.CreateUserAsync();

        var nonExistingProductId = Guid.NewGuid();

        var faker = new Faker();
        var command = new AddItemToCartCommand(
            userId,
            nonExistingProductId,
            faker.Random.Int(min: 1, max: 10));

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.Error.ShouldBe(ProductErrors.NotFound(nonExistingProductId));
    }


    [Fact]
    public async Task Should_ReturnFailure_WhenNotEnoughQuantity()
    {
        // Arrange
        Guid userId = await Sender.CreateUserAsync();

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
            new Specification ("Color", "Red"),
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
        var command = new AddItemToCartCommand(
            userId,
            productId,
            Quantity + 1);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.Error.ShouldBe(ProductErrors.NotEnoughStock(Quantity));
    }

    [Fact]
    public async Task Should_ReturnSuccess_WhenItemAddedToCart()
    {
        // Arrange
        Guid userId = await Sender.CreateUserAsync();

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
            new Specification ("Color", "Red"),
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

        var command = new AddItemToCartCommand(
            userId,
            productId,
            Quantity);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task User_ShouldBeAbleTo_AddProductToCart()
    {
        var faker = new Faker();
        var createCategoryCommand = new CreateCategoryCommand(faker.Commerce.Categories(1)[0], null, null, null);
        Result<Guid> createCategoryResult = await Sender.Send(createCategoryCommand);
        createCategoryResult.IsSuccess.ShouldBeTrue();
        Guid categoryId = createCategoryResult.Value;

        var createCarModelCommand = new CreateCarModelCommand(
            faker.Vehicle.Manufacturer(),
            faker.Vehicle.Model(),
            faker.Date.Past(10).Year,
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel),
            null
        );

        Result<Guid> createCarModelResult = await Sender.Send(createCarModelCommand);
        createCarModelResult.IsSuccess.ShouldBeTrue();
        Guid carModelId = createCarModelResult.Value;

        var specifications = new List<Specification>
        {
            new Specification ("Color", "Red"),
            new Specification ("Engine", "V8")
        };


        var createProductCommand = new CreateProductCommand(
            faker.Commerce.ProductName(),
            specifications,
            faker.Commerce.Ean13(),
            categoryId,
            carModelId,
            faker.Random.Decimal(100, 1000),
            faker.Random.Decimal(0, 100),
            Quantity
        );

        Result<Guid> createProductResult = await Sender.Send(createProductCommand);


        createProductResult.IsSuccess.ShouldBeTrue();
        Guid productId = createProductResult.Value;

        var createUserCommand = new RegisterUserCommand(faker.Internet.Email(), faker.Name.FirstName(), faker.Name.FirstName(), faker.Internet.Password());
        Result<Guid> createUserResult = await Sender.Send(createUserCommand);
        createUserResult.IsSuccess.ShouldBeTrue();
        Guid userId = createUserResult.Value;

        Result addToCartResult = await Sender.Send(new AddItemToCartCommand(userId, productId, Quantity));
        addToCartResult.IsSuccess.ShouldBeTrue();
    }
}
