using Bogus;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Carts;
using EuroMotors.Application.Carts.AddItemToCart;
using EuroMotors.Application.Carts.GetCartById;
using EuroMotors.Application.Carts.RemoveItemFromCart;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Products;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Carts;

public class RemoveItemFromCartTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public RemoveItemFromCartTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_RemoveItemFromCart_WhenItemExists()
    {
        // Arrange
        await CleanDatabaseAsync();

        // Create products
        Guid categoryId = await ServiceProvider.CreateCategoryAsync(_faker.Commerce.Categories(1)[0]);
        Guid brandId = await ServiceProvider.CreateCarBrandAsync(_faker.Vehicle.Manufacturer());
        Guid carModelId = await ServiceProvider.CreateCarModelAsync(
            brandId,
            _faker.Vehicle.Model(),
            2020,
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel)
        );

        var specifications = new List<Specification>
        {
            new Specification("Color", "Red")
        };

        // Create two products
        Guid product1Id = await ServiceProvider.CreateProductAsync(
            "Product 1",
            "VC1",
            categoryId,
            carModelId,
            100m,
            0m,
            10,
            specifications
        );

        Guid product2Id = await ServiceProvider.CreateProductAsync(
            "Product 2",
            "VC2",
            categoryId,
            carModelId,
            50m,
            0m,
            10,
            specifications
        );

        // Create cart and add both products
        var cartId = Guid.NewGuid();
        ICommandHandler<AddItemToCartCommand> addItemHandler = ServiceProvider.GetRequiredService<ICommandHandler<AddItemToCartCommand>>();

        // Add both products to cart
        await addItemHandler.Handle(new AddItemToCartCommand(cartId, product1Id, 2), CancellationToken.None);
        await addItemHandler.Handle(new AddItemToCartCommand(cartId, product2Id, 1), CancellationToken.None);

        // Verify both items are in the cart
        IQueryHandler<GetCartByIdQuery, Cart> cartQueryHandler = ServiceProvider.GetRequiredService<IQueryHandler<GetCartByIdQuery, Cart>>();
        Result<Cart> initialCartResult = await cartQueryHandler.Handle(new GetCartByIdQuery(cartId), CancellationToken.None);
        initialCartResult.Value.CartItems.Count.ShouldBe(2);

        // Create remove item command
        var command = new RemoveItemFromCartCommand(cartId, product1Id);

        // Act
        ICommandHandler<RemoveItemFromCartCommand> removeHandler = ServiceProvider.GetRequiredService<ICommandHandler<RemoveItemFromCartCommand>>();
        Result result = await removeHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        // Verify item was removed
        Result<Cart> updatedCartResult = await cartQueryHandler.Handle(new GetCartByIdQuery(cartId), CancellationToken.None);
        updatedCartResult.Value.CartItems.Count.ShouldBe(1);
        updatedCartResult.Value.CartItems.ShouldNotContain(item => item.ProductId == product1Id);
        updatedCartResult.Value.CartItems.ShouldContain(item => item.ProductId == product2Id);
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenProductDoesNotExist()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var nonExistentProductId = Guid.NewGuid();
        var command = new RemoveItemFromCartCommand(cartId, nonExistentProductId);

        // Act
        ICommandHandler<RemoveItemFromCartCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<RemoveItemFromCartCommand>>();
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBe(ProductErrors.NotFound(nonExistentProductId));
    }

    [Fact]
    public async Task Should_DoNothing_WhenItemNotInCart()
    {
        // Arrange
        await CleanDatabaseAsync();

        // Create a product
        Guid categoryId = await ServiceProvider.CreateCategoryAsync(_faker.Commerce.Categories(1)[0]);
        Guid brandId = await ServiceProvider.CreateCarBrandAsync(_faker.Vehicle.Manufacturer());
        Guid carModelId = await ServiceProvider.CreateCarModelAsync(
            brandId,
            _faker.Vehicle.Model(),
            2020,
            BodyType.Sedan,
            new EngineSpec(6, FuelType.Diesel)
        );

        var specifications = new List<Specification>
        {
            new Specification("Color", "Red")
        };

        // Create product but don't add it to cart
        Guid productId = await ServiceProvider.CreateProductAsync(
            "Test Product",
            "VC123",
            categoryId,
            carModelId,
            100m,
            0m,
            10,
            specifications
        );

        // Create empty cart
        var cartId = Guid.NewGuid();

        // Create remove command for product not in cart
        var command = new RemoveItemFromCartCommand(cartId, productId);

        // Act
        ICommandHandler<RemoveItemFromCartCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<RemoveItemFromCartCommand>>();
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert - operation should succeed even though item wasn't in cart
        result.IsSuccess.ShouldBeTrue();

        // Verify cart is still empty
        IQueryHandler<GetCartByIdQuery, Cart> cartQueryHandler = ServiceProvider.GetRequiredService<IQueryHandler<GetCartByIdQuery, Cart>>();
        Result<Cart> cartResult = await cartQueryHandler.Handle(new GetCartByIdQuery(cartId), CancellationToken.None);
        cartResult.Value.CartItems.ShouldBeEmpty();
    }
}
