using EuroMotors.Application.Carts;
using EuroMotors.Application.Carts.AddItemToCart;
using EuroMotors.Application.Carts.ClearCart;
using EuroMotors.Application.Carts.ConvertToOrder;
using EuroMotors.Application.Carts.GetCartBySessionId;
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

    [HttpGet("{id}/user")]
    public async Task<IActionResult> GetCartByUserId(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCartByUserIdQuery(id);

        Result<CartResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : NotFound();
    }

    [HttpGet("{id}/session")]
    public async Task<IActionResult> GetCartBySessionId(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCartBySessionIdQuery(id);

        Result<CartResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> AddItemToCart(AddItemToCartRequest request, CancellationToken cancellationToken)
    {
        if (!request.UserId.HasValue && !request.SessionId.HasValue)
        {
            request.SessionId = Guid.NewGuid();
        }

        var command = new AddItemToCartCommand(request.UserId, request.SessionId, request.ProductId, request.Quantity);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest();
    }

    [HttpPost("convert-to-order")]
    public async Task<IActionResult> ConvertToOrder(Guid? userId, Guid? sessionId, CancellationToken cancellationToken)
    {
        var command = new ConvertToOrderCommand(userId ?? Guid.Empty, sessionId);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPatch("/items/quantity")]
    public async Task<IActionResult> UpdateQuantity(Guid? userId, Guid? sessionId, Guid productId, int quantity, CancellationToken cancellationToken)
    {
        var command = new UpdateCartItemQuantityCommand(userId ?? Guid.Empty, sessionId, productId, quantity);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpDelete("/items")]
    public async Task<IActionResult> RemoveItemFromCart(Guid? userId, Guid? sessionId, Guid productId, CancellationToken cancellationToken)
    {
        var command = new RemoveItemFromCartCommand(userId ?? Guid.Empty, sessionId, productId);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpDelete("/clear")]
    public async Task<IActionResult> ClearCart(Guid? userId, Guid? sessionId, CancellationToken cancellationToken)
    {
        var command = new ClearCartCommand(userId ?? Guid.Empty, sessionId);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}

