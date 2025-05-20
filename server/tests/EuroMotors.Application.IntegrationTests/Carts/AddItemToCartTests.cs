using Bogus;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Carts;
using EuroMotors.Application.Carts.AddItemToCart;
using EuroMotors.Application.Carts.GetCartById;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Products;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Carts;

public class AddItemToCartTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public AddItemToCartTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_AddItemToCart_WhenProductExists()
    {
        // Arrange
        await CleanDatabaseAsync();

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
            new Specification("Color", "Red"),
            new Specification("Engine", "V8")
        };

        Guid productId = await ServiceProvider.CreateProductAsync(
            "Test Product",
            "VendorCode123",
            categoryId,
            carModelId,
            100m,
            10m,
            10, // Stock
            specifications
        );

        // Create a cart ID
        var cartId = Guid.NewGuid();
        int quantity = 2;
        
        var command = new AddItemToCartCommand(cartId, productId, quantity);

        // Act
        ICommandHandler<AddItemToCartCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<AddItemToCartCommand>>();
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        // Verify item was added to cart
        IQueryHandler<GetCartByIdQuery, Cart> cartQueryHandler = ServiceProvider.GetRequiredService<IQueryHandler<GetCartByIdQuery, Cart>>();
        Result<Cart> cartResult = await cartQueryHandler.Handle(new GetCartByIdQuery(cartId), CancellationToken.None);
        
        cartResult.IsSuccess.ShouldBeTrue();
        cartResult.Value.Id.ShouldBe(cartId);
        cartResult.Value.CartItems.ShouldNotBeEmpty();
        cartResult.Value.CartItems.Count.ShouldBe(1);
        
        CartItem cartItem = cartResult.Value.CartItems[0];
        cartItem.ProductId.ShouldBe(productId);
        cartItem.Quantity.ShouldBe(quantity);
        cartItem.UnitPrice.ShouldBe(100m); // Price from the created product
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenProductDoesNotExist()
    {
        // Arrange
        await CleanDatabaseAsync();
        
        var cartId = Guid.NewGuid();
        var nonExistentProductId = Guid.NewGuid();
        int quantity = 1;
        
        var command = new AddItemToCartCommand(cartId, nonExistentProductId, quantity);

        // Act
        ICommandHandler<AddItemToCartCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<AddItemToCartCommand>>();
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBe(ProductErrors.NotFound(nonExistentProductId));
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenQuantityExceedsStock()
    {
        // Arrange
        await CleanDatabaseAsync();

        // Create a product with limited stock
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
            new Specification("Color", "Red"),
            new Specification("Engine", "V8")
        };

        const int availableStock = 5;
        Guid productId = await ServiceProvider.CreateProductAsync(
            "Test Product with Limited Stock",
            "VendorCode456",
            categoryId,
            carModelId,
            100m,
            10m,
            availableStock, // Limited stock
            specifications
        );

        // Create a cart and try to add more than available stock
        var cartId = Guid.NewGuid();
        int quantity = availableStock + 1; // Exceeds available stock
        
        var command = new AddItemToCartCommand(cartId, productId, quantity);

        // Act
        ICommandHandler<AddItemToCartCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<AddItemToCartCommand>>();
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldBe(ProductErrors.NotEnoughStock(availableStock));
    }

    [Fact]
    public async Task Should_AddMoreQuantity_WhenAddingSameProductTwice()
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
            new Specification("Color", "Red"),
            new Specification("Engine", "V8")
        };

        Guid productId = await ServiceProvider.CreateProductAsync(
            "Test Product",
            "VendorCode789",
            categoryId,
            carModelId,
            100m,
            10m,
            10, // Stock
            specifications
        );

        // Create a cart ID
        var cartId = Guid.NewGuid();
        
        // Add product to cart first time
        var firstCommand = new AddItemToCartCommand(cartId, productId, 2);
        ICommandHandler<AddItemToCartCommand> handler = ServiceProvider.GetRequiredService<ICommandHandler<AddItemToCartCommand>>();
        Result firstResult = await handler.Handle(firstCommand, CancellationToken.None);
        firstResult.IsSuccess.ShouldBeTrue();
        
        // Add the same product to cart second time
        var secondCommand = new AddItemToCartCommand(cartId, productId, 3);
        Result secondResult = await handler.Handle(secondCommand, CancellationToken.None);
        
        // Assert
        secondResult.IsSuccess.ShouldBeTrue();

        // Verify cart has updated quantity
        IQueryHandler<GetCartByIdQuery, Cart> cartQueryHandler = ServiceProvider.GetRequiredService<IQueryHandler<GetCartByIdQuery, Cart>>();
        Result<Cart> cartResult = await cartQueryHandler.Handle(new GetCartByIdQuery(cartId), CancellationToken.None);
        
        cartResult.IsSuccess.ShouldBeTrue();
        cartResult.Value.CartItems.Count.ShouldBe(1);
        cartResult.Value.CartItems[0].Quantity.ShouldBe(5); // 2 + 3
    }
} 
