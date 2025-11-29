namespace Domain.Configurations;

public class EmailConfiguration
{
    public static readonly string ConfigurationSectionName = "EmailConfiguration";
    public required string SmtpServer { get; init; }
    public required int SmtpPort { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
}