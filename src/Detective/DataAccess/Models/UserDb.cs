using Domain.Enums;

namespace DataAccess.Models;

public class UserDb(string username,
    string password,
    string email,
    UserType type)
{
    public string Username = username;
    public string Password = password;
    public string Email = email;
    public UserType Type = type;

    public ICollection<CharacteristicDb> Characteristics;
}

public class TwoFactorCodeDb(string username,
    string code,
    DateTimeOffset expiresAt,
    int failedAttempts)
{
    public string Username { get; } = username;

    public string Code { get; } = code;

    public DateTimeOffset ExpiresAt { get; } = expiresAt;

    public int FailedAttempts { get; set; } = failedAttempts;
}