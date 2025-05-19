using System.Security.Claims;
using EuroMotors.Api.Controllers.Users;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Users.Login;
using EuroMotors.Application.Users.Logout;
using EuroMotors.Application.Users.Register;
using EuroMotors.Application.Users.Update;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace EuroMotors.Api.UnitTests.Controllers.Users;

public sealed class UsersControllerTests
{
    private readonly UsersController _controller;
    private readonly ICommandHandler<LoginUserCommand, AuthenticationResponse> _loginHandler;
    private readonly ICommandHandler<RegisterUserCommand, Guid> _registerHandler;
    private readonly ICommandHandler<LogoutUserCommand> _logoutHandler;
    private readonly ICommandHandler<UpdateUserInformationCommand> _updateHandler;

    public UsersControllerTests()
    {
        _loginHandler = Substitute.For<ICommandHandler<LoginUserCommand, AuthenticationResponse>>();
        _registerHandler = Substitute.For<ICommandHandler<RegisterUserCommand, Guid>>();
        _logoutHandler = Substitute.For<ICommandHandler<LogoutUserCommand>>();
        _updateHandler = Substitute.For<ICommandHandler<UpdateUserInformationCommand>>();

        _controller = new UsersController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new LoginRequest("test@example.com", "password123");
        var authResponse = new AuthenticationResponse
        {
            AccessToken = "accessToken",
            RefreshToken = "refreshToken"
        };

        _loginHandler.Handle(Arg.Any<LoginUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(authResponse));

        // Act
        IActionResult result = await _controller.Login(request, _loginHandler, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(authResponse);

        await _loginHandler.Received(1).Handle(
            Arg.Is<LoginUserCommand>(cmd => 
                cmd.Email == request.Email && 
                cmd.Password == request.Password),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenCredentialsAreInvalid()
    {
        // Arrange
        var request = new LoginRequest("test@example.com", "wrongpassword");
        var error = Error.Failure("Auth.InvalidCredentials", "Invalid credentials");

        _loginHandler.Handle(Arg.Any<LoginUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<AuthenticationResponse>(error));

        // Act
        IActionResult result = await _controller.Login(request, _loginHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);

        await _loginHandler.Received(1).Handle(
            Arg.Is<LoginUserCommand>(cmd => 
                cmd.Email == request.Email && 
                cmd.Password == request.Password),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenRegistrationSucceeds()
    {
        // Arrange
        var request = new RegisterRequest(
            "test@example.com", 
            "John", 
            "Doe", 
            "password123"
        );
        var userId = Guid.NewGuid();

        _registerHandler.Handle(Arg.Any<RegisterUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(userId));

        // Act
        IActionResult result = await _controller.Register(request, _registerHandler, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(userId);

        await _registerHandler.Received(1).Handle(
            Arg.Is<RegisterUserCommand>(cmd => 
                cmd.Email == request.Email && 
                cmd.FirstName == request.FirstName && 
                cmd.LastName == request.LastName && 
                cmd.Password == request.Password),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenRegistrationFails()
    {
        // Arrange
        var request = new RegisterRequest(
            "test@example.com", 
            "John", 
            "Doe", 
            "password123"
        );
        var error = Error.Conflict("User.EmailExists", "Email already exists");

        _registerHandler.Handle(Arg.Any<RegisterUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Guid>(error));

        // Act
        IActionResult result = await _controller.Register(request, _registerHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);

        await _registerHandler.Received(1).Handle(
            Arg.Is<RegisterUserCommand>(cmd => 
                cmd.Email == request.Email && 
                cmd.FirstName == request.FirstName && 
                cmd.LastName == request.LastName && 
                cmd.Password == request.Password),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Logout_ShouldReturnNoContent_WhenLogoutSucceeds()
    {
        // Arrange
        _logoutHandler.Handle(Arg.Any<LogoutUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.Logout(_logoutHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();

        await _logoutHandler.Received(1).Handle(
            Arg.Any<LogoutUserCommand>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Logout_ShouldReturnBadRequest_WhenLogoutFails()
    {
        // Arrange
        var error = Error.Failure("Auth.LogoutFailed", "Logout failed");
        _logoutHandler.Handle(Arg.Any<LogoutUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.Logout(_logoutHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);

        await _logoutHandler.Received(1).Handle(
            Arg.Any<LogoutUserCommand>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnNoContent_WhenUpdateSucceeds()
    {
        // Arrange
        var request = new UpdateUserInformationRequest(
            "John", 
            "Doe", 
            "1234567890", 
            "New York"
        );
        string email = "test@example.com";

        _updateHandler.Handle(Arg.Any<UpdateUserInformationCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email)
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // Act
        IActionResult result = await _controller.UpdateUser(request, _updateHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();

        await _updateHandler.Received(1).Handle(
            Arg.Is<UpdateUserInformationCommand>(cmd => 
                cmd.Email == email && 
                cmd.FirstName == request.Firstname && 
                cmd.LastName == request.LastName && 
                cmd.PhoneNumber == request.PhoneNumber && 
                cmd.City == request.City),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnUnauthorized_WhenEmailNotInToken()
    {
        // Arrange
        var request = new UpdateUserInformationRequest(
            "John", 
            "Doe", 
            "1234567890", 
            "New York"
        );

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
        };

        // Act
        IActionResult result = await _controller.UpdateUser(request, _updateHandler, CancellationToken.None);

        // Assert
        UnauthorizedObjectResult unauthorizedResult = result.ShouldBeOfType<UnauthorizedObjectResult>();
        
        // Convert the anonymous object to a comparable type
        var expectedObj = new { error = "Email not found in token" };
        string resultJson = System.Text.Json.JsonSerializer.Serialize(unauthorizedResult.Value);
        string expectedJson = System.Text.Json.JsonSerializer.Serialize(expectedObj);
        resultJson.ShouldBe(expectedJson);

        await _updateHandler.DidNotReceive().Handle(
            Arg.Any<UpdateUserInformationCommand>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnBadRequest_WhenUpdateFails()
    {
        // Arrange
        var request = new UpdateUserInformationRequest(
            "John", 
            "Doe", 
            "1234567890", 
            "New York"
        );
        string email = "test@example.com";
        var error = Error.Failure("User.UpdateFailed", "Update failed");

        _updateHandler.Handle(Arg.Any<UpdateUserInformationCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email)
        };
        var identity = new ClaimsIdentity(claims);
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };

        // Act
        IActionResult result = await _controller.UpdateUser(request, _updateHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);

        await _updateHandler.Received(1).Handle(
            Arg.Is<UpdateUserInformationCommand>(cmd => 
                cmd.Email == email && 
                cmd.FirstName == request.Firstname && 
                cmd.LastName == request.LastName && 
                cmd.PhoneNumber == request.PhoneNumber && 
                cmd.City == request.City),
            Arg.Any<CancellationToken>());
    }
} 
