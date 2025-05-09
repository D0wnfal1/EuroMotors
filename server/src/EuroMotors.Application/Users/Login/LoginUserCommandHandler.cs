using System.Globalization;
using EuroMotors.Application.Abstractions.Authentication;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace EuroMotors.Application.Users.Login;

internal sealed class LoginUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider,
    ITokenEncryptionService tokenEncryptionService,
    IHttpContextAccessor httpContextAccessor,
    IUnitOfWork unitOfWork,
    IConfiguration configuration) : ICommandHandler<LoginUserCommand, AuthenticationResponse>
{
    public async Task<Result<AuthenticationResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailAsync(command.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);
        }

        bool verified = passwordHasher.Verify(command.Password, user.PasswordHash);

        if (!verified)
        {
            return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);
        }

        string accessToken = tokenProvider.Create(user);
        (string refreshToken, DateTime refreshTokenExpiry) = tokenProvider.CreateRefreshToken();

        string encryptedRefreshToken = tokenEncryptionService.EncryptToken(refreshToken);
        user.SetRefreshToken(encryptedRefreshToken, refreshTokenExpiry);
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        HttpResponse? response = httpContextAccessor.HttpContext?.Response;
        if (response is not null)
        {
            int expirationInMinutes = Convert.ToInt32(configuration["Jwt:ExpirationInMinutes"], CultureInfo.InvariantCulture);
            int refreshTokenExpirationInDays = Convert.ToInt32(configuration["Jwt:RefreshTokenExpirationInDays"], CultureInfo.InvariantCulture);

            response.Cookies.Append("AccessToken", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(expirationInMinutes)
            });

            response.Cookies.Append("RefreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(refreshTokenExpirationInDays)
            });
        }

        var result = new AuthenticationResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return Result.Success(result);
    }
}
