using System.Security.Claims;
using EuroMotors.Application.Users.GetByEmail;
using EuroMotors.Application.Users.Login;
using EuroMotors.Application.Users.Logout;
using EuroMotors.Application.Users.Register;
using EuroMotors.Application.Users.Update;
using EuroMotors.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Users;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("email")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByEmail(CancellationToken cancellationToken)
    {
        if (User?.Identity is null || !User.Identity.IsAuthenticated)
        {
            return Ok(null);
        }

        string? email = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized("Email not found in token");
        }

        var query = new GetUserByEmailQuery(email);

        Result<UserResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("auth-status")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult GetAuthState()
    {
        return Ok(new { IsAuthenticated = User.Identity?.IsAuthenticated ?? false });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginUserCommand(request.Email, request.Password);

        Result<string> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request.Email, request.FirstName, request.LastName, request.Password);

        Result<Guid> result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var command = new LogoutUserCommand();

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPut]
    [Authorize]
    [Route("update")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserInformationRequest request, CancellationToken cancellationToken)
    {
        string? email = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized("Email not found in token");
        }

        var command = new UpdateUserInformationCommand(email, request.Firstname, request.LastName, request.PhoneNumber, request.City);

        Result result = await _sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
