using System.Security.Claims;
using EuroMotors.Api.Controllers.Users;
using EuroMotors.Application.Users.Login;
using EuroMotors.Application.Users.RefreshToken;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Api.UnitTests.Controllers.Users;

public class AuthControllerTests
{
    private readonly ISender _sender;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _sender = Substitute.For<ISender>();
        _controller = new AuthController(_sender)
        {
            // Set up HttpContext for the controller
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnOk_WhenRefreshTokenSucceeds()
    {
        // Arrange
        string refreshToken = "valid-refresh-token";

        var context = new DefaultHttpContext();
        context.Request.Headers["Cookie"] = $"RefreshToken={refreshToken}";
        _controller.ControllerContext.HttpContext = context;

        var authResponse = new AuthenticationResponse
        {
            AccessToken = "new-access-token",
            RefreshToken = "new-refresh-token"
        };

        _sender.Send(Arg.Any<RefreshTokenCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(authResponse));

        // Act
        IActionResult result = await _controller.RefreshToken(CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(authResponse);

        await _sender.Received(1).Send(
            Arg.Is<RefreshTokenCommand>(cmd => cmd.RefreshToken == refreshToken),
            Arg.Any<CancellationToken>());
    }


    [Fact]
    public async Task RefreshToken_ShouldReturnBadRequest_WhenRefreshTokenFails()
    {
        // Arrange
        string refreshToken = "invalid-refresh-token";

        var context = new DefaultHttpContext();
        context.Request.Headers["Cookie"] = $"RefreshToken={refreshToken}";
        _controller.ControllerContext.HttpContext = context;

        var error = Error.Failure("Token.Invalid", "The refresh token is invalid");

        _sender.Send(Arg.Any<RefreshTokenCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<AuthenticationResponse>(error));

        // Act
        IActionResult result = await _controller.RefreshToken(CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnNoContent_WhenRefreshTokenIsEmpty()
    {
        // Arrange - no refresh token in request

        // Act
        IActionResult result = await _controller.RefreshToken(CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();
        await _sender.DidNotReceive().Send(Arg.Any<RefreshTokenCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public void GetAuthState_ShouldReturnAuthStatus_WhenCalled()
    {
        // Arrange - unauthenticated by default

        // Act
        IActionResult result = _controller.GetAuthState();

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        AuthState value = okResult.Value.ShouldBeOfType<AuthState>();
        Assert.False(value.IsAuthenticated);

        // Test authenticated scenario
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "test") }, "test");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext.User = principal;

        // Act again
        result = _controller.GetAuthState();

        // Assert authenticated
        okResult = result.ShouldBeOfType<OkObjectResult>();
        value = okResult.Value.ShouldBeOfType<AuthState>();
        Assert.True(value.IsAuthenticated);
    }
}
