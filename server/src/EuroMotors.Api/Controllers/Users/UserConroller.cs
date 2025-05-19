using System.Security.Claims;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Users.Login;
using EuroMotors.Application.Users.Logout;
using EuroMotors.Application.Users.Register;
using EuroMotors.Application.Users.Update;
using EuroMotors.Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Users;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, ICommandHandler<LoginUserCommand, AuthenticationResponse> handler, CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(request.Email, request.Password);

        Result<AuthenticationResponse> result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, ICommandHandler<RegisterUserCommand, Guid> handler, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request.Email, request.FirstName, request.LastName, request.Password);

        Result<Guid> result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(ICommandHandler<LogoutUserCommand> handler, CancellationToken cancellationToken)
    {
        var command = new LogoutUserCommand();

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPut]
    [Authorize]
    [Route("update")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserInformationRequest request, ICommandHandler<UpdateUserInformationCommand> handler, CancellationToken cancellationToken)
    {
        string? email = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized(new { error = "Email not found in token" });
        }

        var command = new UpdateUserInformationCommand(email, request.Firstname, request.LastName, request.PhoneNumber, request.City);

        Result result = await handler.Handle(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
