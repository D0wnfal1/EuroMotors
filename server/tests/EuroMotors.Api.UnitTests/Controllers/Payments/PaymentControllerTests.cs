using EuroMotors.Api.Controllers.Payments;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Payments.CreatePayment;
using EuroMotors.Application.Payments.GetPaymentById;
using EuroMotors.Application.Payments.GetPaymentByOrderId;
using EuroMotors.Application.Payments.GetPaymentByStatus;
using EuroMotors.Domain.Payments;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Api.UnitTests.Controllers.Payments;

public class PaymentControllerTests
{
    private readonly PaymentController _controller;
    private readonly IQueryHandler<GetPaymentByIdQuery, PaymentResponse> _getPaymentByIdHandler;
    private readonly IQueryHandler<GetPaymentByOrderIdQuery, PaymentResponse> _getPaymentByOrderIdHandler;
    private readonly IQueryHandler<GetPaymentByStatusQuery, PaymentResponse> _getPaymentByStatusHandler;
    private readonly ICommandHandler<CreatePaymentCommand, Dictionary<string, string>> _createPaymentHandler;

    public PaymentControllerTests()
    {
        _getPaymentByIdHandler = Substitute.For<IQueryHandler<GetPaymentByIdQuery, PaymentResponse>>();
        _getPaymentByOrderIdHandler = Substitute.For<IQueryHandler<GetPaymentByOrderIdQuery, PaymentResponse>>();
        _getPaymentByStatusHandler = Substitute.For<IQueryHandler<GetPaymentByStatusQuery, PaymentResponse>>();
        _createPaymentHandler = Substitute.For<ICommandHandler<CreatePaymentCommand, Dictionary<string, string>>>();

        _controller = new PaymentController()
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task GetPaymentById_ShouldReturnOk_WhenPaymentFound()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var payment = new PaymentResponse(
            paymentId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            PaymentStatus.Success,
            100.00m,
            DateTime.UtcNow
        );

        _getPaymentByIdHandler.Handle(Arg.Any<GetPaymentByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(payment));

        // Act
        IActionResult result = await _controller.GetPaymentById(_getPaymentByIdHandler, paymentId, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(payment);

        await _getPaymentByIdHandler.Received(1).Handle(
            Arg.Is<GetPaymentByIdQuery>(query => query.PaymentId == paymentId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPaymentById_ShouldReturnNotFound_WhenPaymentNotFound()
    {
        // Arrange
        var paymentId = Guid.NewGuid();

        _getPaymentByIdHandler.Handle(Arg.Any<GetPaymentByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<PaymentResponse>(Error.NotFound("Payment.NotFound", "Payment not found")));

        // Act
        IActionResult result = await _controller.GetPaymentById(_getPaymentByIdHandler, paymentId, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetPaymentByOrderId_ShouldReturnOk_WhenPaymentFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var payment = new PaymentResponse(
            Guid.NewGuid(),
            orderId,
            Guid.NewGuid(),
            PaymentStatus.Success,
            100.00m,
            DateTime.UtcNow
        );

        _getPaymentByOrderIdHandler.Handle(Arg.Any<GetPaymentByOrderIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(payment));

        // Act
        IActionResult result = await _controller.GetPaymentByOrderId(_getPaymentByOrderIdHandler, orderId, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(payment);

        await _getPaymentByOrderIdHandler.Received(1).Handle(
            Arg.Is<GetPaymentByOrderIdQuery>(query => query.OrderId == orderId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPaymentByOrderId_ShouldReturnNotFound_WhenPaymentNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _getPaymentByOrderIdHandler.Handle(Arg.Any<GetPaymentByOrderIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<PaymentResponse>(Error.NotFound("Payment.NotFound", "Payment for order not found")));

        // Act
        IActionResult result = await _controller.GetPaymentByOrderId(_getPaymentByOrderIdHandler, orderId, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetPaymentByStatus_ShouldReturnOk_WhenPaymentsFound()
    {
        // Arrange
        PaymentStatus status = PaymentStatus.Success;
        var payment = new PaymentResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            status,
            100.00m,
            DateTime.UtcNow
        );

        _getPaymentByStatusHandler.Handle(Arg.Any<GetPaymentByStatusQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(payment));

        // Act
        IActionResult result = await _controller.GetPaymentByStatus(_getPaymentByStatusHandler, status, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(payment);

        await _getPaymentByStatusHandler.Received(1).Handle(
            Arg.Is<GetPaymentByStatusQuery>(query => query.Status == status),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetPaymentByStatus_ShouldReturnNotFound_WhenPaymentsNotFound()
    {
        // Arrange
        PaymentStatus status = PaymentStatus.Pending;

        _getPaymentByStatusHandler.Handle(Arg.Any<GetPaymentByStatusQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<PaymentResponse>(Error.NotFound("Payments.NotFound", "Payments with specified status not found")));

        // Act
        IActionResult result = await _controller.GetPaymentByStatus(_getPaymentByStatusHandler, status, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreatePayment_ShouldReturnOk_WhenCreationSucceeds()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var paymentData = new Dictionary<string, string>
        {
            { "data", "base64encodeddata" },
            { "signature", "paymentsignature" }
        };

        _createPaymentHandler.Handle(Arg.Any<CreatePaymentCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(paymentData));

        // Act
        IActionResult result = await _controller.CreatePayment(orderId, _createPaymentHandler, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(paymentData);

        await _createPaymentHandler.Received(1).Handle(
            Arg.Is<CreatePaymentCommand>(cmd => cmd.OrderId == orderId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreatePayment_ShouldReturnBadRequest_WhenCreationFails()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var error = Error.Failure("Payment.CreationFailed", "Failed to create payment");

        _createPaymentHandler.Handle(Arg.Any<CreatePaymentCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Dictionary<string, string>>(error));

        // Act
        IActionResult result = await _controller.CreatePayment(orderId, _createPaymentHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }
}
