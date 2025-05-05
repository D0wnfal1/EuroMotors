using EuroMotors.Api.Controllers.Carts;
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
    private readonly ISender _sender;
    private readonly CartController _controller;

    public CartControllerTests()
    {
        _sender = Substitute.For<ISender>();
        _controller = new CartController(_sender)
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
        _sender.Send(Arg.Any<GetCartByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Cart>(Error.NotFound("Cart.NotFound", "Cart not found")));

        // Act
        IActionResult result = await _controller.GetCartById(id, CancellationToken.None);

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

        _sender.Send(Arg.Any<AddItemToCartCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.AddItemToCart(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<OkResult>();

        await _sender.Received(1).Send(
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
        _sender.Send(Arg.Any<AddItemToCartCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.AddItemToCart(request, CancellationToken.None);

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

        _sender.Send(Arg.Any<UpdateItemQuantityCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.UpdateItemQuantity(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<OkResult>();

        await _sender.Received(1).Send(
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
        _sender.Send(Arg.Any<UpdateItemQuantityCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.UpdateItemQuantity(request, CancellationToken.None);

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

        _sender.Send(Arg.Any<RemoveItemFromCartCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.RemoveItemFromCart(cartId, productId, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<OkResult>();

        await _sender.Received(1).Send(
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
        _sender.Send(Arg.Any<RemoveItemFromCartCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.RemoveItemFromCart(cartId, productId, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task ClearCart_ShouldReturnOk_WhenClearingSucceeds()
    {
        // Arrange
        var cartId = Guid.NewGuid();

        _sender.Send(Arg.Any<ClearCartCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.ClearCart(cartId, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<OkResult>();

        await _sender.Received(1).Send(
            Arg.Is<ClearCartCommand>(cmd => cmd.CartId == cartId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ClearCart_ShouldReturnBadRequest_WhenClearingFails()
    {
        // Arrange
        var cartId = Guid.NewGuid();

        var error = Error.Failure("Cart.NotFound", "Cart not found");
        _sender.Send(Arg.Any<ClearCartCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.ClearCart(cartId, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }
}
