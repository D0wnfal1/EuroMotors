using EuroMotors.Application.Carts;
using EuroMotors.Application.Carts.AddItemToCart;
using EuroMotors.Application.Carts.ClearCart;
using EuroMotors.Application.Carts.GetCartById;
using EuroMotors.Application.Carts.RemoveItemFromCart;
using EuroMotors.Application.Carts.UpdateItemQuantity;
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

        Result<Cart> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> AddItemToCart(AddItemToCartRequest request, CancellationToken cancellationToken)
    {
        var command = new AddItemToCartCommand(request.CartId, request.ProductId, request.Quantity);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost("update-quantity")]
    public async Task<IActionResult> UpdateItemQuantity(UpdateItemQuantityRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateItemQuantityCommand(request.CartId, request.ProductId, request.Quantity);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }


    [HttpDelete("item")]
    public async Task<IActionResult> RemoveItemFromCart(Guid cartId, Guid productId, CancellationToken cancellationToken)
    {
        var command = new RemoveItemFromCartCommand(cartId, productId);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart(Guid cartId, CancellationToken cancellationToken)
    {
        var command = new ClearCartCommand(cartId);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}

