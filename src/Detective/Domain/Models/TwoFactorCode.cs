namespace Domain.Models;

public class TwoFactorCode(string username, string code, DateTimeOffset expiresAt, int attempts)
{
    public string Username { get; } = username;
    public string Code{ get; } = code;
    public DateTimeOffset ExpiresAt{ get; } = expiresAt;
    public int  FailedAttempts{ get; } = attempts;
}