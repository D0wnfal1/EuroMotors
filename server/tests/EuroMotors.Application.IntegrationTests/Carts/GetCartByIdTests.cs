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

public class GetCartByIdTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public GetCartByIdTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnEmptyCart_WhenNewCartId()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var query = new GetCartByIdQuery(cartId);

        // Act
        IQueryHandler<GetCartByIdQuery, Cart> handler = ServiceProvider.GetRequiredService<IQueryHandler<GetCartByIdQuery, Cart>>();
        Result<Cart> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(cartId);
        result.Value.CartItems.ShouldBeEmpty();
    }

    [Fact]
    public async Task Should_ReturnCartWithItems_WhenItemsAdded()
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
            "VendorCode123",
            categoryId,
            carModelId,
            100m,
            10m,
            10, // Stock
            specifications
        );

        // Create a second product
        Guid secondProductId = await ServiceProvider.CreateProductAsync(
            "Second Test Product",
            "VendorCode456",
            categoryId,
            carModelId,
            200m,
            20m,
            5, // Stock
            specifications
        );

        // Create a cart and add items
        var cartId = Guid.NewGuid();
        
        // Add first product
        ICommandHandler<AddItemToCartCommand> addFirstItemHandler = ServiceProvider.GetRequiredService<ICommandHandler<AddItemToCartCommand>>();
        await addFirstItemHandler.Handle(new AddItemToCartCommand(cartId, productId, 2), CancellationToken.None);
        
        // Add second product
        await addFirstItemHandler.Handle(new AddItemToCartCommand(cartId, secondProductId, 1), CancellationToken.None);
        
        // Create the query
        var query = new GetCartByIdQuery(cartId);

        // Act
        IQueryHandler<GetCartByIdQuery, Cart> handler = ServiceProvider.GetRequiredService<IQueryHandler<GetCartByIdQuery, Cart>>();
        Result<Cart> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(cartId);
        
        // Check items
        result.Value.CartItems.Count.ShouldBe(2);
        
        CartItem? firstItem = result.Value.CartItems.FirstOrDefault(i => i.ProductId == productId);
        firstItem.ShouldNotBeNull();
        firstItem.Quantity.ShouldBe(2);
        firstItem.UnitPrice.ShouldBe(100m);
        
        CartItem? secondItem = result.Value.CartItems.FirstOrDefault(i => i.ProductId == secondProductId);
        secondItem.ShouldNotBeNull();
        secondItem.Quantity.ShouldBe(1);
        secondItem.UnitPrice.ShouldBe(200m);
    }

    [Fact]
    public async Task Should_CalculateTotalCorrectly()
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

        // Product 1: $100 x 3 = $300
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

        // Product 2: $50 x 2 = $100
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

        // Create cart and add items
        var cartId = Guid.NewGuid();
        ICommandHandler<AddItemToCartCommand> addItemHandler = ServiceProvider.GetRequiredService<ICommandHandler<AddItemToCartCommand>>();
        await addItemHandler.Handle(new AddItemToCartCommand(cartId, product1Id, 3), CancellationToken.None);
        await addItemHandler.Handle(new AddItemToCartCommand(cartId, product2Id, 2), CancellationToken.None);

        // Act
        IQueryHandler<GetCartByIdQuery, Cart> handler = ServiceProvider.GetRequiredService<IQueryHandler<GetCartByIdQuery, Cart>>();
        Result<Cart> result = await handler.Handle(new GetCartByIdQuery(cartId), CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        
        // Total should be $300 + $100 = $400
        decimal expectedTotal = 400m;
        result.Value.TotalPrice.ShouldBe(expectedTotal);
    }
} 
