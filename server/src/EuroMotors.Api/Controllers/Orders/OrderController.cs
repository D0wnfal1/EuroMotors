using EuroMotors.Application.Orders.ChangeOrderStatus;
using EuroMotors.Application.Orders.DeleteOrder;
using EuroMotors.Application.Orders.GetOrderById;
using EuroMotors.Application.Orders.GetOrders;
using EuroMotors.Application.Orders.GetUserOrders;
using EuroMotors.Application.Orders.SetPayment;
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

        return result.IsSuccess ? Ok(result) : NotFound();
    }

    [HttpGet("{id}/user")]
    public async Task<IActionResult> GetUserOrders(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetUserOrdersQuery(id);

        Result<IReadOnlyCollection<OrdersResponse>> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(CancellationToken cancellationToken)
    {
        var query = new GetOrdersQuery();

        Result<IReadOnlyCollection<OrdersResponse>> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result) : NotFound();
    }

    [HttpPost("{id}/set-payment")]
    public async Task<IActionResult> SetPayment(Guid id, [FromBody] SetPaymentRequest request)
    {
        var command = new SetPaymentCommand(id, request.PaymentId, request.Status);

        Result result = await _sender.Send(command);

        return result.IsSuccess ? Ok() : BadRequest(result.Error);
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

