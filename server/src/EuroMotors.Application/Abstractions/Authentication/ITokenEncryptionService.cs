namespace EuroMotors.Application.Abstractions.Authentication;

public interface ITokenEncryptionService
{
    string EncryptToken(string token);
    string DecryptToken(string encryptedToken);
}