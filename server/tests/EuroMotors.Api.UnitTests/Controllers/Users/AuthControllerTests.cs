using System.Collections;
using System.Security.Claims;
using EuroMotors.Api.Controllers.Users;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Users.GetByEmail;
using EuroMotors.Application.Users.Login;
using EuroMotors.Application.Users.RefreshToken;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace EuroMotors.Api.UnitTests.Controllers.Users;

public sealed class AuthControllerTests
{
    private readonly AuthController _controller;
    private readonly ICommandHandler<RefreshTokenCommand, AuthenticationResponse> _refreshTokenHandler;
    private readonly IQueryHandler<GetUserByEmailQuery, UserResponse> _getUserHandler;

    public AuthControllerTests()
    {
        _refreshTokenHandler = Substitute.For<ICommandHandler<RefreshTokenCommand, AuthenticationResponse>>();
        _getUserHandler = Substitute.For<IQueryHandler<GetUserByEmailQuery, UserResponse>>();

        _controller = new AuthController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnOk_WhenTokenIsValid()
    {
        // Arrange
        string refreshToken = "valid-refresh-token";
        var authResponse = new AuthenticationResponse
        {
            AccessToken = "new-access-token",
            RefreshToken = "new-refresh-token"
        };

        _refreshTokenHandler.Handle(Arg.Any<RefreshTokenCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(authResponse));
        
        // Setup cookie in request
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = $"RefreshToken={refreshToken}";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        IActionResult result = await _controller.RefreshToken(_refreshTokenHandler, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(authResponse);

        await _refreshTokenHandler.Received(1).Handle(
            Arg.Is<RefreshTokenCommand>(cmd => cmd.RefreshToken == refreshToken),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnNoContent_WhenNoTokenProvided()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        // Using empty TestRequestCookieCollection
        httpContext.Request.Cookies = new TestRequestCookieCollection();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        IActionResult result = await _controller.RefreshToken(_refreshTokenHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();

        await _refreshTokenHandler.DidNotReceive().Handle(
            Arg.Any<RefreshTokenCommand>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnUnauthorized_WhenTokenIsInvalid()
    {
        // Arrange
        string refreshToken = "invalid-refresh-token";
        var error = Error.Failure("Auth.InvalidToken", "Invalid refresh token");

        _refreshTokenHandler.Handle(Arg.Any<RefreshTokenCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<AuthenticationResponse>(error));

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["Cookie"] = $"RefreshToken={refreshToken}";
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        IActionResult result = await _controller.RefreshToken(_refreshTokenHandler, CancellationToken.None);

        // Assert
        UnauthorizedObjectResult unauthorizedResult = result.ShouldBeOfType<UnauthorizedObjectResult>();
        
        var expectedObj = new { error };
        string resultJson = JsonSerializer.Serialize(unauthorizedResult.Value);
        string expectedJson = JsonSerializer.Serialize(expectedObj);
        
        resultJson.ShouldContain(error.Code);
        resultJson.ShouldContain(error.Description);
        resultJson.ShouldContain("\"Type\":0");

        await _refreshTokenHandler.Received(1).Handle(
            Arg.Is<RefreshTokenCommand>(cmd => cmd.RefreshToken == refreshToken),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAuthState_ShouldReturnUnauthenticatedState_WhenUserIsNotAuthenticated()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        IActionResult result = await _controller.GetAuthState(_getUserHandler, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        AuthState authState = okResult.Value.ShouldBeOfType<AuthState>();
        authState.IsAuthenticated.ShouldBeFalse();
        authState.User.ShouldBeNull();

        await _getUserHandler.DidNotReceive().Handle(
            Arg.Any<GetUserByEmailQuery>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAuthState_ShouldReturnUserState_WhenUserIsAuthenticated()
    {
        // Arrange
        string email = "test@example.com";
        var userResponse = new UserResponse
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = "John",
            LastName = "Doe",
            Roles = new List<string>()
        };

        _getUserHandler.Handle(Arg.Any<GetUserByEmailQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(userResponse));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        IActionResult result = await _controller.GetAuthState(_getUserHandler, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        AuthState authState = okResult.Value.ShouldBeOfType<AuthState>();
        authState.IsAuthenticated.ShouldBeTrue();
        authState.User.ShouldBe(userResponse);

        await _getUserHandler.Received(1).Handle(
            Arg.Is<GetUserByEmailQuery>(query => query.Email == email),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAuthState_ShouldReturnUnauthenticatedState_WhenUserHasNoEmailClaim()
    {
        // Arrange
        var identity = new ClaimsIdentity("TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        IActionResult result = await _controller.GetAuthState(_getUserHandler, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        AuthState authState = okResult.Value.ShouldBeOfType<AuthState>();
        authState.IsAuthenticated.ShouldBeFalse();
        authState.User.ShouldBeNull();

        await _getUserHandler.DidNotReceive().Handle(
            Arg.Any<GetUserByEmailQuery>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAuthState_ShouldReturnUnauthenticatedState_WhenUserNotFound()
    {
        // Arrange
        string email = "test@example.com";
        var error = Error.NotFound("User.NotFound", "User not found");

        _getUserHandler.Handle(Arg.Any<GetUserByEmailQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<UserResponse>(error));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext
        {
            User = principal
        };
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        IActionResult result = await _controller.GetAuthState(_getUserHandler, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        AuthState authState = okResult.Value.ShouldBeOfType<AuthState>();
        authState.IsAuthenticated.ShouldBeTrue();
        authState.User.ShouldBeNull();

        await _getUserHandler.Received(1).Handle(
            Arg.Is<GetUserByEmailQuery>(query => query.Email == email),
            Arg.Any<CancellationToken>());
    }
}

internal sealed class TestRequestCookieCollection : Dictionary<string, string>, IRequestCookieCollection
{
    public new ICollection<string> Keys => base.Keys;

    public new string this[string key] => 
        key != null && TryGetValue(key, out string value) ? value : string.Empty;

    public new bool ContainsKey(string key) => 
        key != null && base.ContainsKey(key);

    public new IEnumerator<KeyValuePair<string, string>> GetEnumerator() => base.GetEnumerator();

    public new bool TryGetValue(string key, out string value)
    {
        if (base.TryGetValue(key, out string? tempValue))
        {
            value = tempValue;
            return true;
        }

        value = string.Empty; 
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
} 
