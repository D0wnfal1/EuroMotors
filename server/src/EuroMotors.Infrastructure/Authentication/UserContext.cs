using System.Security.Claims;
using EuroMotors.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Infrastructure.Authentication;

internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId =>
        _httpContextAccessor
            .HttpContext?
            .User
            .GetUserId() ??
        throw new ApplicationException("User context is unavailable");

    public List<string> Roles =>
        _httpContextAccessor
            .HttpContext?
            .User
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList() ?? [];
}
