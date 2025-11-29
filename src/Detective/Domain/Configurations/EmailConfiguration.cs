namespace Domain.Configurations;

public class EmailConfiguration
{
    public static readonly string ConfigurationSectionName = "EmailConfiguration";
    public string SmtpServer { get; init; }
    public int SmtpPort { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
}