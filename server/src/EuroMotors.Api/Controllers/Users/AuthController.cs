using System.Security.Claims;
using System.Security.Principal;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Users.GetByEmail;
using EuroMotors.Application.Users.Login;
using EuroMotors.Application.Users.RefreshToken;
using EuroMotors.Domain.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Users;

[Route("api/auth")]
[ApiController]
public sealed class AuthController : ControllerBase
{
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(ICommandHandler<RefreshTokenCommand, AuthenticationResponse> handler, CancellationToken cancellationToken)
    {
        string? refreshToken = Request.Cookies["RefreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return NoContent();
        }

        var query = new RefreshTokenCommand(refreshToken);

        Result<AuthenticationResponse> result = await handler.Handle(query, cancellationToken);

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
    public async Task<IActionResult> GetAuthState(IQueryHandler<GetUserByEmailQuery, UserResponse> handler, CancellationToken cancellationToken)
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
        Result<UserResponse> result = await handler.Handle(query, cancellationToken);

        return Ok(new AuthState
        {
            IsAuthenticated = true,
            User = result.IsSuccess ? result.Value : null
        });
    }
}

