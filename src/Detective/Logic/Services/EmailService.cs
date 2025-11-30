using Domain.Configurations;
using Domain.Interfaces.Service;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Logic.Services;

public class EmailService : IEmailService
{
    private readonly EmailConfiguration _emailOptions;

    public EmailService(IOptions<EmailConfiguration> emailOptions)
    {
        _emailOptions = emailOptions.Value;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Detective API", _emailOptions.Email));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();

        await client.ConnectAsync(_emailOptions.SmtpServer, _emailOptions.SmtpPort);

        await client.AuthenticateAsync(_emailOptions.Email, _emailOptions.Password);

        await client.SendAsync(message);

        await client.DisconnectAsync(true);
    }
}