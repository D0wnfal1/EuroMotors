using System.Security.Claims;
using EuroMotors.Api.Controllers.Users;
using EuroMotors.Application.Users.GetByEmail;
using EuroMotors.Application.Users.Login;
using EuroMotors.Application.Users.Logout;
using EuroMotors.Application.Users.Register;
using EuroMotors.Application.Users.Update;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Api.UnitTests.Controllers.Users;

public class UsersControllerTests
{
    private readonly ISender _sender;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _sender = Substitute.For<ISender>();
        _controller = new UsersController(_sender)
        {
            // Set up HttpContext for the controller
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task GetByEmail_ShouldReturnOk_WhenUserFound()
    {
        // Arrange
        string email = "test@example.com";
        var identity = new ClaimsIdentity(new Claim[] 
        { 
            new Claim(ClaimTypes.Email, email)
        }, "test");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext.User = principal;
        
        var userResponse = new UserResponse
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = "John",
            LastName = "Doe"
        };
        
        _sender.Send(Arg.Any<GetUserByEmailQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(userResponse));

        // Act
        IActionResult result = await _controller.GetByEmail(CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(userResponse);
        
        await _sender.Received(1).Send(
            Arg.Is<GetUserByEmailQuery>(query => query.Email == email), 
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByEmail_ShouldReturnOkWithNull_WhenUserNotAuthenticated()
    {
        // Arrange - unauthenticated user

        // Act
        IActionResult result = await _controller.GetByEmail(CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBeNull();
        
        await _sender.DidNotReceive().Send(
            Arg.Any<GetUserByEmailQuery>(), 
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetByEmail_ShouldReturnUnauthorized_WhenEmailNotInClaims()
    {
        // Arrange
        var identity = new ClaimsIdentity(new Claim[] 
        { 
            new Claim(ClaimTypes.Name, "user")
        }, "test");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext.User = principal;

        // Act
        IActionResult result = await _controller.GetByEmail(CancellationToken.None);

        // Assert
        UnauthorizedObjectResult unauthorizedResult = result.ShouldBeOfType<UnauthorizedObjectResult>();
        unauthorizedResult.Value.ShouldBe("Email not found in token");
        
        await _sender.DidNotReceive().Send(
            Arg.Any<GetUserByEmailQuery>(), 
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Login_ShouldReturnOk_WhenLoginSucceeds()
    {
        // Arrange
        var request = new LoginRequest("test@example.com", "password123");
        
        var authResponse = new AuthenticationResponse
        {
            AccessToken = "access-token",
            RefreshToken = "refresh-token"
        };
        
        _sender.Send(Arg.Any<LoginUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(authResponse));

        // Act
        IActionResult result = await _controller.Login(request, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(authResponse);
        
        await _sender.Received(1).Send(
            Arg.Is<LoginUserCommand>(cmd => 
                cmd.Email == request.Email && 
                cmd.Password == request.Password), 
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenLoginFails()
    {
        // Arrange
        var request = new LoginRequest("test@example.com", "wrong-password");
        
        var error = Error.Failure("User.InvalidCredentials", "Invalid email or password");
        _sender.Send(Arg.Any<LoginUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<AuthenticationResponse>(error));

        // Act
        IActionResult result = await _controller.Login(request, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenRegistrationSucceeds()
    {
        // Arrange
        var request = new RegisterRequest("new@example.com", "Jane", "Smith", "password123");
        
        var userId = Guid.NewGuid();
        _sender.Send(Arg.Any<RegisterUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(userId));

        // Act
        IActionResult result = await _controller.Register(request, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(userId);
        
        await _sender.Received(1).Send(
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
        var request = new RegisterRequest("existing@example.com", "Jane", "Smith", "password123");
        
        var error = Error.Conflict("User.AlreadyExists", "A user with this email already exists");
        _sender.Send(Arg.Any<RegisterUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Guid>(error));

        // Act
        IActionResult result = await _controller.Register(request, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task Logout_ShouldReturnNoContent_WhenLogoutSucceeds()
    {
        // Arrange
        _sender.Send(Arg.Any<LogoutUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.Logout(CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();
        
        await _sender.Received(1).Send(
            Arg.Any<LogoutUserCommand>(), 
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Logout_ShouldReturnBadRequest_WhenLogoutFails()
    {
        // Arrange
        var error = Error.Failure("Logout.Failed", "Failed to process logout");
        _sender.Send(Arg.Any<LogoutUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.Logout(CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnNoContent_WhenUpdateSucceeds()
    {
        // Arrange
        string email = "test@example.com";
        var identity = new ClaimsIdentity(new Claim[] 
        { 
            new Claim(ClaimTypes.Email, email)
        }, "test");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext.User = principal;

        var request = new UpdateUserInformationRequest("UpdatedFirst", "UpdatedLast", "1234567890", "New York");
        
        _sender.Send(Arg.Any<UpdateUserInformationCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.UpdateUser(request, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();
        
        await _sender.Received(1).Send(
            Arg.Is<UpdateUserInformationCommand>(cmd => 
                cmd.Email == email && 
                cmd.FirstName == request.Firstname &&
                cmd.LastName == request.LastName &&
                cmd.PhoneNumber == request.PhoneNumber &&
                cmd.City == request.City), 
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnUnauthorized_WhenEmailNotInClaims()
    {
        // Arrange
        var identity = new ClaimsIdentity(new Claim[] 
        { 
            new Claim(ClaimTypes.Name, "user")
        }, "test");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext.User = principal;
        
        var request = new UpdateUserInformationRequest("UpdatedFirst", "UpdatedLast", "1234567890", "New York");

        // Act
        IActionResult result = await _controller.UpdateUser(request, CancellationToken.None);

        // Assert
        UnauthorizedObjectResult unauthorizedResult = result.ShouldBeOfType<UnauthorizedObjectResult>();
        unauthorizedResult.Value.ShouldBe("Email not found in token");
        
        await _sender.DidNotReceive().Send(
            Arg.Any<UpdateUserInformationCommand>(), 
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnBadRequest_WhenUpdateFails()
    {
        // Arrange
        string email = "test@example.com";
        var identity = new ClaimsIdentity(new Claim[] 
        { 
            new Claim(ClaimTypes.Email, email)
        }, "test");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext.User = principal;

        var request = new UpdateUserInformationRequest("UpdatedFirst", "UpdatedLast", "1234567890", "New York");

        var error = Error.Failure("User.NotFound", "User not found");
        _sender.Send(Arg.Any<UpdateUserInformationCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.UpdateUser(request, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }
} 
