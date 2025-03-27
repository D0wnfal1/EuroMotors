using System.Globalization;
using EuroMotors.Application.Abstractions.Authentication;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Users.Login;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace EuroMotors.Application.Users.RefreshToken;

internal sealed class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    ITokenProvider tokenProvider,
    IUnitOfWork unitOfWork,
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration
) : ICommandHandler<RefreshTokenCommand, AuthenticationResponse>
{
    public async Task<Result<AuthenticationResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByRefreshTokenAsync(command.RefreshToken, cancellationToken);

        if (user is null)
        {
            return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);
        }

        if (user.RefreshTokenExpiryTime is null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);
        }

        string newAccessToken = tokenProvider.Create(user);
        (string newRefreshToken, DateTime newRefreshTokenExpiry) = tokenProvider.CreateRefreshToken();

        user.SetRefreshToken(newRefreshToken, newRefreshTokenExpiry);
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        HttpResponse? response = httpContextAccessor.HttpContext?.Response;
        if (response is not null)
        {
            int expirationInMinutes = Convert.ToInt32(configuration["Jwt:ExpirationInMinutes"], CultureInfo.InvariantCulture);
            int refreshTokenExpirationInDays = Convert.ToInt32(configuration["Jwt:RefreshTokenExpirationInDays"], CultureInfo.InvariantCulture);

            response.Cookies.Append("AccessToken", newAccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(expirationInMinutes)
            });

            response.Cookies.Append("RefreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(refreshTokenExpirationInDays)
            });
        }

        var result = new AuthenticationResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };

        return Result.Success(result);
    }
}
