using EuroMotors.Api.Controllers.Carts;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Carts;
using EuroMotors.Application.Carts.AddItemToCart;
using EuroMotors.Application.Carts.ClearCart;
using EuroMotors.Application.Carts.GetCartById;
using EuroMotors.Application.Carts.RemoveItemFromCart;
using EuroMotors.Application.Carts.UpdateItemQuantity;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Api.UnitTests.Controllers.Carts;

public class CartControllerTests
{
    private readonly CartController _controller;
    private readonly IQueryHandler<GetCartByIdQuery, Cart> _getCartByIdHandler;
    private readonly ICommandHandler<AddItemToCartCommand> _addItemToCartHandler;
    private readonly ICommandHandler<UpdateItemQuantityCommand> _updateItemQuantityHandler;
    private readonly ICommandHandler<RemoveItemFromCartCommand> _removeItemFromCartHandler;
    private readonly ICommandHandler<ClearCartCommand> _clearCartHandler;

    public CartControllerTests()
    {
        _getCartByIdHandler = Substitute.For<IQueryHandler<GetCartByIdQuery, Cart>>();
        _addItemToCartHandler = Substitute.For<ICommandHandler<AddItemToCartCommand>>();
        _updateItemQuantityHandler = Substitute.For<ICommandHandler<UpdateItemQuantityCommand>>();
        _removeItemFromCartHandler = Substitute.For<ICommandHandler<RemoveItemFromCartCommand>>();
        _clearCartHandler = Substitute.For<ICommandHandler<ClearCartCommand>>();

        _controller = new CartController()
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task GetCartById_ShouldReturnNotFound_WhenCartNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _getCartByIdHandler.Handle(Arg.Any<GetCartByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Cart>(Error.NotFound("Cart.NotFound", "Cart not found")));

        // Act
        IActionResult result = await _controller.GetCartById(_getCartByIdHandler, id, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task AddItemToCart_ShouldReturnOk_WhenAddingSucceeds()
    {
        // Arrange
        var request = new AddItemToCartRequest
        {
            CartId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Quantity = 5
        };

        _addItemToCartHandler.Handle(Arg.Any<AddItemToCartCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.AddItemToCart(request, _addItemToCartHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<OkResult>();

        await _addItemToCartHandler.Received(1).Handle(
            Arg.Is<AddItemToCartCommand>(cmd =>
                cmd.CartId == request.CartId &&
                cmd.ProductId == request.ProductId &&
                cmd.Quantity == request.Quantity),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddItemToCart_ShouldReturnBadRequest_WhenAddingFails()
    {
        // Arrange
        var request = new AddItemToCartRequest
        {
            CartId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Quantity = 5
        };

        var error = Error.Failure("Product.NotFound", "Product not found");
        _addItemToCartHandler.Handle(Arg.Any<AddItemToCartCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.AddItemToCart(request, _addItemToCartHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task UpdateItemQuantity_ShouldReturnOk_WhenUpdateSucceeds()
    {
        // Arrange
        var request = new UpdateItemQuantityRequest
        {
            CartId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Quantity = 3
        };

        _updateItemQuantityHandler.Handle(Arg.Any<UpdateItemQuantityCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.UpdateItemQuantity(request, _updateItemQuantityHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<OkResult>();

        await _updateItemQuantityHandler.Received(1).Handle(
            Arg.Is<UpdateItemQuantityCommand>(cmd =>
                cmd.CartId == request.CartId &&
                cmd.ProductId == request.ProductId &&
                cmd.Quantity == request.Quantity),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateItemQuantity_ShouldReturnBadRequest_WhenUpdateFails()
    {
        // Arrange
        var request = new UpdateItemQuantityRequest
        {
            CartId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            Quantity = 3
        };

        var error = Error.Failure("Cart.ItemNotFound", "Cart item not found");
        _updateItemQuantityHandler.Handle(Arg.Any<UpdateItemQuantityCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.UpdateItemQuantity(request, _updateItemQuantityHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task RemoveItemFromCart_ShouldReturnOk_WhenRemovalSucceeds()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        _removeItemFromCartHandler.Handle(Arg.Any<RemoveItemFromCartCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.RemoveItemFromCart(cartId, productId, _removeItemFromCartHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<OkResult>();

        await _removeItemFromCartHandler.Received(1).Handle(
            Arg.Is<RemoveItemFromCartCommand>(cmd =>
                cmd.CartId == cartId &&
                cmd.ProductId == productId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RemoveItemFromCart_ShouldReturnBadRequest_WhenRemovalFails()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var error = Error.Failure("Cart.ItemNotFound", "Cart item not found");
        _removeItemFromCartHandler.Handle(Arg.Any<RemoveItemFromCartCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.RemoveItemFromCart(cartId, productId, _removeItemFromCartHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task ClearCart_ShouldReturnOk_WhenClearingSucceeds()
    {
        // Arrange
        var cartId = Guid.NewGuid();

        _clearCartHandler.Handle(Arg.Any<ClearCartCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.ClearCart(cartId, _clearCartHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<OkResult>();

        await _clearCartHandler.Received(1).Handle(
            Arg.Is<ClearCartCommand>(cmd => cmd.CartId == cartId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ClearCart_ShouldReturnBadRequest_WhenClearingFails()
    {
        // Arrange
        var cartId = Guid.NewGuid();

        var error = Error.Failure("Cart.NotFound", "Cart not found");
        _clearCartHandler.Handle(Arg.Any<ClearCartCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.ClearCart(cartId, _clearCartHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }
}
