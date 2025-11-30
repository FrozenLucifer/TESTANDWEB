using System.Security.Cryptography;
using Domain.Interfaces;

namespace Logic;

public class PasswordProvider : IPasswordProvider
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string hash, string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public string GenerateTemporaryPassword(int length)
    {
        const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var chars = new char[length];

        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = validChars[RandomNumberGenerator.GetInt32(validChars.Length)];
        }

        return new string(chars);    }
}