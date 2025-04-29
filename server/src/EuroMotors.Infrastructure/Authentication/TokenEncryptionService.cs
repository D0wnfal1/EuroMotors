using System.Security.Cryptography;
using System.Text;
using EuroMotors.Application.Abstractions.Authentication;
using Microsoft.Extensions.Configuration;

namespace EuroMotors.Infrastructure.Authentication;

internal sealed class TokenEncryptionService : ITokenEncryptionService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public TokenEncryptionService(IConfiguration configuration)
    {
        string encryptionKey = configuration["Jwt:EncryptionKey"] ??
            throw new InvalidOperationException("Encryption key is not configured");

        string encryptionIV = configuration["Jwt:EncryptionIV"] ??
            throw new InvalidOperationException("Encryption IV is not configured");

        _key = SHA256.HashData(Encoding.UTF8.GetBytes(encryptionKey));
#pragma warning disable CA5351
        _iv = MD5.HashData(Encoding.UTF8.GetBytes(encryptionIV));
#pragma warning restore CA5351
    }

    public string EncryptToken(string token)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

#pragma warning disable CA5401
        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
#pragma warning restore CA5401

        using MemoryStream msEncrypt = new();
        using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (StreamWriter swEncrypt = new(csEncrypt))
        {
            swEncrypt.Write(token);
        }

        return Convert.ToBase64String(msEncrypt.ToArray());
    }

    public string DecryptToken(string encryptedToken)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using MemoryStream msDecrypt = new(Convert.FromBase64String(encryptedToken));
        using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
        using StreamReader srDecrypt = new(csDecrypt);

        return srDecrypt.ReadToEnd();
    }
}
