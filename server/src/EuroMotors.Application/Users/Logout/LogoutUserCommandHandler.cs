using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Application.Users.Logout;

internal sealed class LogoutUserCommandHandler(IHttpContextAccessor httpContextAccessor) : ICommandHandler<LogoutUserCommand>
{
    public Task<Result> Handle(LogoutUserCommand command, CancellationToken cancellationToken)
    {
        if (httpContextAccessor.HttpContext?.Request.Cookies["AccessToken"] == null &&
            httpContextAccessor.HttpContext?.Request.Cookies["RefreshToken"] == null)
        {
            return Task.FromResult(Result.Success());
        }

        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(-1),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        };

        httpContextAccessor.HttpContext.Response.Cookies.Append("AccessToken", string.Empty, cookieOptions);
        httpContextAccessor.HttpContext.Response.Cookies.Append("RefreshToken", string.Empty, cookieOptions);

        return Task.FromResult(Result.Success());
    }
}
