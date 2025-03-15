using EuroMotors.Application.Orders.ChangeOrderStatus;
using EuroMotors.Application.Orders.CreateOrder;
using EuroMotors.Application.Orders.DeleteOrder;
using EuroMotors.Application.Orders.GetOrderById;
using EuroMotors.Application.Orders.GetOrders;
using EuroMotors.Application.Orders.GetUserOrders;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Orders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Orders;

[Route("api/orders")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly ISender _sender;

    public OrderController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);

        Result<OrderResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("{id}/user")]
    public async Task<IActionResult> GetUserOrders(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetUserOrdersQuery(id);

        Result<IReadOnlyCollection<OrdersResponse>> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(CancellationToken cancellationToken)
    {
        var query = new GetOrdersQuery();

        Result<IReadOnlyCollection<OrdersResponse>> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody]CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand(request.CartId, request.UserId, request.DeliveryMethod, request.ShippingAddress, request.PaymentMethod); 

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(new { orderId = result.Value }) : BadRequest();
    }

    [HttpPatch]
    public async Task<IActionResult> ChangeOrderStatus(Guid id, OrderStatus status, CancellationToken cancellationToken)
    {
        var command = new ChangeOrderStatusCommand(id, status);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : NotFound();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteOrder(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteOrderCommand(id);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : NotFound();
    }
}

