using EuroMotors.Application.Carts;
using EuroMotors.Application.Carts.AddItemToCart;
using EuroMotors.Application.Carts.ClearCart;
using EuroMotors.Application.Carts.ConvertToOrder;
using EuroMotors.Application.Carts.GetCartByUserId;
using EuroMotors.Application.Carts.RemoveItemFromCart;
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

    [HttpGet("{id}/user")]
    public async Task<IActionResult> GetCartByUserId(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCartByUserIdQuery(id);

        Result<Cart> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> AddItemToCart(AddItemToCartRequest request, CancellationToken cancellationToken)
    {
        var command = new AddItemToCartCommand(request.UserId, request.ProductId, request.Quantity);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest();
    }

    [HttpPost("convert-to-order")]
    public async Task<IActionResult> ConvertToOrder(Guid userId, CancellationToken cancellationToken)
    {
        var command = new ConvertToOrderCommand(userId);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpDelete("/items")]
    public async Task<IActionResult> RemoveItemFromCart(Guid userId, Guid productId, CancellationToken cancellationToken)
    {
        var command = new RemoveItemFromCartCommand(userId, productId);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpDelete("/clear")]
    public async Task<IActionResult> ClearCart(Guid userId, CancellationToken cancellationToken)
    {
        var command = new ClearCartCommand(userId);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}

