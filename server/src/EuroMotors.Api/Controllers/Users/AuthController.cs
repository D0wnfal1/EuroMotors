using System.Security.Claims;
using System.Security.Principal;
using EuroMotors.Application.Users.GetByEmail;
using EuroMotors.Application.Users.Login;
using EuroMotors.Application.Users.RefreshToken;
using EuroMotors.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Users;

[Route("api/auth")]
[ApiController]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
    {
        string? refreshToken = Request.Cookies["RefreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { error = "No refresh token provided" });
        }

        var query = new RefreshTokenCommand(refreshToken);

        Result<AuthenticationResponse> result = await _sender.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            Response.Cookies.Delete("RefreshToken");
            Response.Cookies.Delete("AccessToken");

            return Unauthorized(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpGet("status")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthState), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAuthState(CancellationToken cancellationToken)
    {
        IIdentity? identity = User.Identity;

        if (identity?.IsAuthenticated != true)
        {
            return Ok(new AuthState
            {
                IsAuthenticated = false,
                User = null
            });
        }

        string? email = User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
        {
            return Ok(new AuthState
            {
                IsAuthenticated = false,
                User = null
            });
        }

        var query = new GetUserByEmailQuery(email);
        Result<UserResponse> result = await _sender.Send(query, cancellationToken);

        return Ok(new AuthState
        {
            IsAuthenticated = true,
            User = result.IsSuccess ? result.Value : null
        });
    }
}

