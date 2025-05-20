using EuroMotors.Api.Controllers.Orders;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.Orders.ChangeOrderStatus;
using EuroMotors.Application.Orders.CreateOrder;
using EuroMotors.Application.Orders.DeleteOrder;
using EuroMotors.Application.Orders.GetOrderById;
using EuroMotors.Application.Orders.GetOrders;
using EuroMotors.Application.Orders.GetUserOrders;
using EuroMotors.Domain.Orders;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Api.UnitTests.Controllers.Orders;

public class OrderControllerTests
{
    private readonly OrderController _controller;
    private readonly IQueryHandler<GetOrderByIdQuery, OrderResponse> _getOrderByIdHandler;
    private readonly IQueryHandler<GetUserOrdersQuery, IReadOnlyCollection<OrdersResponse>> _getUserOrdersHandler;
    private readonly IQueryHandler<GetOrdersQuery, Pagination<OrdersResponse>> _getOrdersHandler;
    private readonly ICommandHandler<CreateOrderCommand, Guid> _createOrderHandler;
    private readonly ICommandHandler<ChangeOrderStatusCommand> _changeOrderStatusHandler;
    private readonly ICommandHandler<DeleteOrderCommand> _deleteOrderHandler;

    public OrderControllerTests()
    {
        _getOrderByIdHandler = Substitute.For<IQueryHandler<GetOrderByIdQuery, OrderResponse>>();
        _getUserOrdersHandler = Substitute.For<IQueryHandler<GetUserOrdersQuery, IReadOnlyCollection<OrdersResponse>>>();
        _getOrdersHandler = Substitute.For<IQueryHandler<GetOrdersQuery, Pagination<OrdersResponse>>>();
        _createOrderHandler = Substitute.For<ICommandHandler<CreateOrderCommand, Guid>>();
        _changeOrderStatusHandler = Substitute.For<ICommandHandler<ChangeOrderStatusCommand>>();
        _deleteOrderHandler = Substitute.For<ICommandHandler<DeleteOrderCommand>>();

        _controller = new OrderController()
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task GetOrderById_ShouldReturnOk_WhenOrderFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new OrderResponse(
            orderId,
            Guid.NewGuid(),
            "John Doe",
            "+380123456789",
            "john@example.com",
            OrderStatus.Pending,
            100.00m,
            DeliveryMethod.Pickup,
            "Some address",
            PaymentMethod.Postpaid,
            DateTime.UtcNow,
            DateTime.UtcNow
        );

        _getOrderByIdHandler.Handle(Arg.Any<GetOrderByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(order));

        // Act
        IActionResult result = await _controller.GetOrderById(_getOrderByIdHandler, orderId, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(order);

        await _getOrderByIdHandler.Received(1).Handle(
            Arg.Is<GetOrderByIdQuery>(query => query.OrderId == orderId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOrderById_ShouldReturnNotFound_WhenOrderNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _getOrderByIdHandler.Handle(Arg.Any<GetOrderByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<OrderResponse>(Error.NotFound("Order.NotFound", "Order not found")));

        // Act
        IActionResult result = await _controller.GetOrderById(_getOrderByIdHandler, orderId, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetUserOrders_ShouldReturnOk_WhenOrdersFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var orders = new List<OrdersResponse>
        {
            new OrdersResponse
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Status = OrderStatus.Pending,
                TotalPrice = 100.00m,
                DeliveryMethod = DeliveryMethod.Pickup,
                ShippingAddress = "Some address",
                PaymentMethod = PaymentMethod.Postpaid,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            }
        };

        _getUserOrdersHandler.Handle(Arg.Any<GetUserOrdersQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<IReadOnlyCollection<OrdersResponse>>(orders));

        // Act
        IActionResult result = await _controller.GetUserOrders(_getUserOrdersHandler, userId, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(orders);

        await _getUserOrdersHandler.Received(1).Handle(
            Arg.Is<GetUserOrdersQuery>(query => query.UserId == userId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetUserOrders_ShouldReturnNotFound_WhenOrdersNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _getUserOrdersHandler.Handle(Arg.Any<GetUserOrdersQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<IReadOnlyCollection<OrdersResponse>>(Error.NotFound("Orders.NotFound", "Orders not found")));

        // Act
        IActionResult result = await _controller.GetUserOrders(_getUserOrdersHandler, userId, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetOrders_ShouldReturnOk_WhenOrdersFound()
    {
        // Arrange
        var orders = new List<OrdersResponse>
        {
            new OrdersResponse
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Status = OrderStatus.Pending,
                TotalPrice = 100.00m,
                DeliveryMethod = DeliveryMethod.Pickup,
                ShippingAddress = "Some address",
                PaymentMethod = PaymentMethod.Postpaid,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            }
        };

        var pagination = new Pagination<OrdersResponse>
        {
            PageIndex = 1,
            PageSize = 10,
            Count = 1,
            Data = orders
        };

        _getOrdersHandler.Handle(Arg.Any<GetOrdersQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(pagination));

        // Act
        IActionResult result = await _controller.GetOrders(_getOrdersHandler, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(pagination);

        await _getOrdersHandler.Received(1).Handle(
            Arg.Is<GetOrdersQuery>(query =>
                query.PageNumber == 1 &&
                query.PageSize == 10 &&
                query.Status == null),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOrders_ShouldPassAllParameters_ToQuery()
    {
        // Arrange
        var pagination = new Pagination<OrdersResponse>
        {
            PageIndex = 2,
            PageSize = 20,
            Count = 0,
            Data = new List<OrdersResponse>()
        };

        _getOrdersHandler.Handle(Arg.Any<GetOrdersQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(pagination));

        OrderStatus status = OrderStatus.Paid;
        int pageNumber = 2;
        int pageSize = 20;

        // Act
        await _controller.GetOrders(_getOrdersHandler, CancellationToken.None, pageNumber, pageSize, status);

        // Assert
        await _getOrdersHandler.Received(1).Handle(
            Arg.Is<GetOrdersQuery>(query =>
                query.PageNumber == pageNumber &&
                query.PageSize == pageSize &&
                query.Status == status),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOrders_ShouldReturnNotFound_WhenOrdersNotFound()
    {
        // Arrange
        _getOrdersHandler.Handle(Arg.Any<GetOrdersQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Pagination<OrdersResponse>>(Error.NotFound("Orders.NotFound", "Orders not found")));

        // Act
        IActionResult result = await _controller.GetOrders(_getOrdersHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnBadRequest_WhenCreationFails()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CartId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BuyerName = "John Doe",
            BuyerPhoneNumber = "+380123456789",
            BuyerEmail = "john@example.com",
            DeliveryMethod = DeliveryMethod.Pickup,
            ShippingAddress = "Some address",
            PaymentMethod = PaymentMethod.Postpaid
        };

        _createOrderHandler.Handle(Arg.Any<CreateOrderCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Guid>(Error.Failure("Order.CreationFailed", "Failed to create order")));

        // Act
        IActionResult result = await _controller.CreateOrder(request, _createOrderHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBeOfType<Error>();
    }

    [Fact]
    public async Task ChangeOrderStatus_ShouldReturnNoContent_WhenStatusChangeSucceeds()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        OrderStatus status = OrderStatus.Paid;

        _changeOrderStatusHandler.Handle(Arg.Any<ChangeOrderStatusCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.ChangeOrderStatus(orderId, status, _changeOrderStatusHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();

        await _changeOrderStatusHandler.Received(1).Handle(
            Arg.Is<ChangeOrderStatusCommand>(cmd =>
                cmd.OrderId == orderId &&
                cmd.Status == status),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ChangeOrderStatus_ShouldReturnNotFound_WhenStatusChangeFails()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        OrderStatus status = OrderStatus.Paid;

        _changeOrderStatusHandler.Handle(Arg.Any<ChangeOrderStatusCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(Error.NotFound("Order.NotFound", "Order not found")));

        // Act
        IActionResult result = await _controller.ChangeOrderStatus(orderId, status, _changeOrderStatusHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task DeleteOrder_ShouldReturnNoContent_WhenDeletionSucceeds()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _deleteOrderHandler.Handle(Arg.Any<DeleteOrderCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.DeleteOrder(orderId, _deleteOrderHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();

        await _deleteOrderHandler.Received(1).Handle(
            Arg.Is<DeleteOrderCommand>(cmd => cmd.OrderId == orderId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteOrder_ShouldReturnNotFound_WhenDeletionFails()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _deleteOrderHandler.Handle(Arg.Any<DeleteOrderCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(Error.NotFound("Order.NotFound", "Order not found")));

        // Act
        IActionResult result = await _controller.DeleteOrder(orderId, _deleteOrderHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }
}
