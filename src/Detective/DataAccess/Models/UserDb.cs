using Domain.Enums;

namespace DataAccess.Models;

public class UserDb(string username,
    string password,
    string email,
    UserType type)
{
    public string Username { get; init; } = username;
    public string Password { get; set; } = password;
    public string Email { get; init; }= email;
    public UserType Type { get; init; }= type;

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