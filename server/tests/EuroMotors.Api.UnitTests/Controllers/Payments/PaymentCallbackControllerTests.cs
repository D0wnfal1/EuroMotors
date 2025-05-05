using EuroMotors.Api.Controllers.Payments;
using EuroMotors.Application.Abstractions.Payments;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Api.UnitTests.Controllers.Payments;

public class PaymentCallbackControllerTests
{
    private readonly IPaymentService _paymentService;
    private readonly PaymentCallbackController _controller;

    public PaymentCallbackControllerTests()
    {
        _paymentService = Substitute.For<IPaymentService>();
        _controller = new PaymentCallbackController(_paymentService)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task PaymentCallback_ShouldReturnOk_WhenCallbackProcessingSucceeds()
    {
        // Arrange
        const string data = "base64encodeddata";
        const string signature = "paymentsignature";

        _paymentService.ProcessPaymentCallbackAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.PaymentCallback(data, signature);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(true);

        await _paymentService.Received(1).ProcessPaymentCallbackAsync(
            Arg.Is<string>(d => d == data),
            Arg.Is<string>(s => s == signature));
    }

    [Fact]
    public async Task PaymentCallback_ShouldReturnBadRequest_WhenCallbackProcessingFails()
    {
        // Arrange
        const string data = "invaliddata";
        const string signature = "invalidsignature";
        var error = Error.Failure("Payment.InvalidCallback", "Invalid payment callback data");

        _paymentService.ProcessPaymentCallbackAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.PaymentCallback(data, signature);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }
}
