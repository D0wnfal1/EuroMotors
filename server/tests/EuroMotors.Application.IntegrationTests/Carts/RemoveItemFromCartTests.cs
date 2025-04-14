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
    public async Task Should_ReturnFailure_WhenProductDoesNotExist()
    {
        //Arrange
        Guid userId = await Sender.CreateUserAsync();

        var command = new RemoveItemFromCartCommand(
            userId,
            Guid.NewGuid());

        //Act
        Result result = await Sender.Send(command);

        //Assert
        result.Error.ShouldBe(ProductErrors.NotFound(command.ProductId));
    }

    [Fact]
    public async Task Should_ReturnSuccess_WhenRemovedItemFromCart()
    {
        // Arrange
        Guid userId = await Sender.CreateUserAsync();

        var faker = new Faker();
        Guid categoryId = await Sender.CreateCategoryAsync(faker.Commerce.Categories(1)[0]);
        Guid carModelId = await Sender.CreateCarModelAsync(
            faker.Vehicle.Manufacturer(),
            faker.Vehicle.Model(),
            2020,
            null,
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel),
            null
        );
        Guid productId = await Sender.CreateProductAsync(
            "Product Name",
            "Product Description",
            "VendorCode123",
            categoryId,
            carModelId,
            100m,
            10m,
            Quantity
        );

        await Sender.CreateProductAsync(
            "Another Product",
            "Another Description",
            "VendorCode456",
            categoryId,
            carModelId,
            200m,
            20m,
            Quantity
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
