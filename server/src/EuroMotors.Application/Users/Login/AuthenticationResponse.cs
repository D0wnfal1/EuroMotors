namespace EuroMotors.Application.Users.Login;

public sealed class AuthenticationResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}

