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
    private readonly Faker _faker = new();

    public AddItemToCartTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenUserDoesNotExist()
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
            _faker.Random.Guid(),
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

        var command = new AddItemToCartCommand(
            userId,
            nonExistingProductId,
            _faker.Random.Int(min: 1, max: 10));

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

        Guid categoryId = await Sender.CreateCategoryAsync("Category Category");
        Guid brandId = await Sender.CreateCarBrandAsync("Test Brand1124");
        Guid carModelId = await Sender.CreateCarModelAsync(
            brandId,
            _faker.Vehicle.Model(),
            _faker.Random.Int(2000, 2023),
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel)
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

        Guid categoryId = await Sender.CreateCategoryAsync("Category1");
        Guid brandId = await Sender.CreateCarBrandAsync("Brand1");
        Guid carModelId = await Sender.CreateCarModelAsync(
            brandId,
            _faker.Vehicle.Model(),
            _faker.Random.Int(2000, 2023),
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel)
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
        var createCategoryCommand = new CreateCategoryCommand("CategoryName1", null, null, null);
        Result<Guid> createCategoryResult = await Sender.Send(createCategoryCommand);
        createCategoryResult.IsSuccess.ShouldBeTrue();
        Guid categoryId = createCategoryResult.Value;

        Guid brandId = await Sender.CreateCarBrandAsync("BrandName1");

        var createCarModelCommand = new CreateCarModelCommand(
            brandId,
            _faker.Vehicle.Model(),
            _faker.Random.Int(2000, 2023),
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel)
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
            _faker.Commerce.ProductName(),
            specifications,
            _faker.Commerce.Ean13(),
            categoryId,
            carModelId,
            _faker.Random.Decimal(100, 1000),
            _faker.Random.Decimal(0, 100),
            Quantity
        );

        Result<Guid> createProductResult = await Sender.Send(createProductCommand);


        createProductResult.IsSuccess.ShouldBeTrue();
        Guid productId = createProductResult.Value;

        var createUserCommand = new RegisterUserCommand(_faker.Internet.Email(), _faker.Name.FirstName(), _faker.Name.FirstName(), _faker.Internet.Password());
        Result<Guid> createUserResult = await Sender.Send(createUserCommand);
        createUserResult.IsSuccess.ShouldBeTrue();
        Guid userId = createUserResult.Value;

        Result addToCartResult = await Sender.Send(new AddItemToCartCommand(userId, productId, Quantity));
        addToCartResult.IsSuccess.ShouldBeTrue();
    }
}
