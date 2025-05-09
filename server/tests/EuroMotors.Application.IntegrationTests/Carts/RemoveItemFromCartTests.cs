using Bogus;
using EuroMotors.Application.Carts.RemoveItemFromCart;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Products;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Carts;

public class RemoveItemFromCartTests : BaseIntegrationTest
{
    private const int Quantity = 10;

    public RemoveItemFromCartTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenUserDoesNotExist()
    {
        //Arrange
        var command = new RemoveItemFromCartCommand(
            Guid.NewGuid(),
            Guid.NewGuid());

        //Act
        Result result = await Sender.Send(command);

        //Assert
        result.Error.Type.ShouldBe(ErrorType.NotFound);
    }

    [Fact]
    public async Task Should_ReturnSuccess_WhenRemovedItemFromCart()
    {
        // Arrange
        Guid userId = await Sender.CreateUserAsync();

        var faker = new Faker();
        Guid brandId = await Sender.CreateCarBrandAsync("BrandName");
        Guid categoryId = await Sender.CreateCategoryAsync("CategoryName2");
        Guid carModelId = await Sender.CreateCarModelAsync(
            brandId,
            faker.Vehicle.Model(),
            2020,
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel)
        );

        var specifications = new List<Specification>
        {
            new Specification ( "Color", "Red" ),
            new Specification ("Engine", "V8")
        };

        Guid productId = await Sender.CreateProductAsync(
            "Product Name",
            "VendorCode456",
            categoryId,
            carModelId,
            100m,
            10m,
            Quantity,
            specifications
        );

        await Sender.CreateProductAsync(
            "Another Product",
            "VendorCode456",
            categoryId,
            carModelId,
            200m,
            20m,
            Quantity,
            specifications
        );

        var command = new RemoveItemFromCartCommand(
            userId,
            productId);

        // Act
        Result result = await Sender.Send(command);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

}
