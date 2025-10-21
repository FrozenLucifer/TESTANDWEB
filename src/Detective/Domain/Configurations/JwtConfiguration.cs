namespace Domain.Configurations;

public class JwtConfiguration
{
    public static readonly string ConfigurationSectionName = "JwtConfiguration";
    public required string Key { get; init; }
    public TimeSpan TokenLifetime { get; init; } = TimeSpan.FromHours(1);
}