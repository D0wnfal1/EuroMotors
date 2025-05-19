using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Orders.ChangeOrderStatus;
using EuroMotors.Application.Orders.CreateOrder;
using EuroMotors.Application.Orders.DeleteOrder;
using EuroMotors.Application.Orders.GetOrderById;
using EuroMotors.Application.Orders.GetOrders;
using EuroMotors.Application.Orders.GetUserOrders;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Orders;

[Route("api/orders")]
[ApiController]
public class OrderController : ControllerBase
{
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOrderById(IQueryHandler<GetOrderByIdQuery, OrderResponse> handler, Guid id, CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdQuery(id);

        Result<OrderResponse> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet("{userId}/user")]
    [Authorize]
    public async Task<IActionResult> GetUserOrders(IQueryHandler<GetUserOrdersQuery, IReadOnlyCollection<OrdersResponse>> handler, Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetUserOrdersQuery(userId);

        Result<IReadOnlyCollection<OrdersResponse>> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> GetOrders(
        IQueryHandler<GetOrdersQuery, Pagination<OrdersResponse>> handler,
        CancellationToken cancellationToken,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] OrderStatus? status = null)
    {
        var query = new GetOrdersQuery(pageNumber, pageSize, status);

        Result<Pagination<OrdersResponse>> result = await handler.Handle(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, ICommandHandler<CreateOrderCommand, Guid> handler, CancellationToken cancellationToken)
    {
        var command = new CreateOrderCommand(request.CartId, request.UserId, request.BuyerName, request.BuyerPhoneNumber, request.BuyerEmail, request.DeliveryMethod, request.ShippingAddress, request.PaymentMethod);

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? Ok(new { orderId = result.Value }) : BadRequest(result.Error);
    }

    [HttpPatch("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> ChangeOrderStatus(Guid id, OrderStatus status, ICommandHandler<ChangeOrderStatusCommand> handler, CancellationToken cancellationToken)
    {
        var command = new ChangeOrderStatusCommand(id, status);

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? NoContent() : NotFound();
    }

    [HttpDelete]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteOrder(Guid id, ICommandHandler<DeleteOrderCommand> handler, CancellationToken cancellationToken)
    {
        var command = new DeleteOrderCommand(id);

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? NoContent() : NotFound();
    }
}

