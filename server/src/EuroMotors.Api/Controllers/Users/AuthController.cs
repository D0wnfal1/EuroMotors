using EuroMotors.Application.Users.Login;
using EuroMotors.Application.Users.RefreshToken;
using EuroMotors.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EuroMotors.Api.Controllers.Users;
    [Route("api/auth")]
    [ApiController]
public class AuthController : ControllerBase
{
	private readonly ISender _sender;

	public AuthController(ISender sender)
	{
		_sender = sender;
	}

	[HttpPost("refresh")]
	public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
	{
        string? refreshToken = Request.Cookies["RefreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return BadRequest("Refresh token not found.");
        }

        var query = new RefreshTokenCommand(refreshToken);

		Result<AuthenticationResponse> result = await _sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
