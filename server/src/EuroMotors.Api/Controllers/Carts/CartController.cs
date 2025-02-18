using EuroMotors.Application.Carts.AddItemToCart;
using EuroMotors.Application.Carts.ClearCart;
using EuroMotors.Application.Carts.ConvertToOrder;
using EuroMotors.Application.Carts.GetCartById;
using EuroMotors.Application.Carts.GetCartByUserId;
using EuroMotors.Application.Carts.RemoveItemFromCart;
using EuroMotors.Application.Carts.UpdateCartItemQuantity;
using EuroMotors.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Carts;

[Route("api/carts")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly ISender _sender;

    public CartController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCartById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCartByIdQuery(id);

        Result<CartResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : NotFound();
    }

    [HttpGet("{id}/user")]
    public async Task<IActionResult> GetCartByUserId(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCartByUserIdQuery(id);

        Result<CartResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : NotFound();
    }

    [HttpGet("{id}/total_price")]
    public async Task<IActionResult> GetCartTotalPrice(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCartByIdQuery(id);
        Result<CartResponse> result = await _sender.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound();
        }

        decimal totalPrice = result.Value.CartItems.Sum(item => item.TotalPrice);
        return Ok(totalPrice);
    }

    [HttpPost]
    public async Task<IActionResult> AddItemToCart(CartRequest request, CancellationToken cancellationToken)
    {
        var command = new AddItemToCartCommand(request.UserId, request.ProductId, request.Quantity);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest();
    }

    [HttpPost("{userId}/convert-to-order")]
    public async Task<IActionResult> ConvertToOrder(Guid userId)
    {
        var command = new ConvertToOrderCommand(userId);

        Result result = await _sender.Send(command);

        return result.IsSuccess ? Ok() : BadRequest();
    }

    [HttpPatch("{userId}/items/{productId}/quantity")]
    public async Task<IActionResult> UpdateQuantity(Guid userId, Guid productId, [FromBody] int quantity, CancellationToken cancellationToken)
    {
        var command = new UpdateCartItemQuantityCommand(userId, productId, quantity);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest();
    }

    [HttpDelete("{userId}/items/{productId}")]
    public async Task<IActionResult> RemoveItemFromCart(Guid userId, Guid productId, CancellationToken cancellationToken)
    {
        var command = new RemoveItemFromCartCommand(userId, productId);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest();
    }

    [HttpDelete("{userId}/clear")]
    public async Task<IActionResult> ClearCart(Guid userId, CancellationToken cancellationToken)
    {
        var command = new ClearCartCommand(userId);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest();
    }
}

