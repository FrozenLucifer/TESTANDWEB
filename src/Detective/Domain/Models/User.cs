using Domain.Enums;

namespace Domain.Models;

public class User(UserType type, string username, string password, string email)
{
    public UserType Type = type;
    public string Username = username;
    public string Password = password;
    public string Email = email;
}

public class TwoFactorCode(string username, string code, DateTimeOffset ExpiresAt, int attempts)
{
    public string Username = username;
    public string Code = code;
    public DateTimeOffset ExpiresAt = ExpiresAt;
    public int  FailedAttempts = attempts;
}