using EuroMotors.Api.Controllers.Callback;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Callback.RequestCallback;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Api.UnitTests.Controllers.Callback;

public class CallbackControllerTests
{
    private readonly ICommandHandler<RequestCallbackCommand> _requestCallbackHandler;
    private readonly CallbackController _controller;

    public CallbackControllerTests()
    {
        _requestCallbackHandler = Substitute.For<ICommandHandler<RequestCallbackCommand>>();
        _controller = new CallbackController()
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task RequestCallback_ShouldReturnOk_WhenCallbackRequestSucceeds()
    {
        // Arrange
        var request = new CallbackRequest
        {
            Name = "John Doe",
            Phone = "+380123456789"
        };

        _requestCallbackHandler.Handle(Arg.Any<RequestCallbackCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.RequestCallback(request, _requestCallbackHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        await _requestCallbackHandler.Received(1).Handle(
            Arg.Is<RequestCallbackCommand>(cmd =>
                cmd.Name == request.Name &&
                cmd.Phone == request.Phone),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RequestCallback_ShouldReturnNotFound_WhenCallbackRequestFails()
    {
        // Arrange
        var request = new CallbackRequest
        {
            Name = "John Doe",
            Phone = "+380123456789"
        };

        _requestCallbackHandler.Handle(Arg.Any<RequestCallbackCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(Error.Failure("Callback.Failed", "Failed to process callback request")));

        // Act
        IActionResult result = await _controller.RequestCallback(request, _requestCallbackHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }
}
