using System.Text.Json;
using System.Text.RegularExpressions;
using BDDTests.Hooks;
using DataAccess;
using DataAccess.Models;
using Domain.Enums;
using Domain.Interfaces;
using DTOs;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Reqnroll;
using RestSharp;

namespace BDDTests.Steps;

[Binding]
public class AuthSteps : IDisposable
{
    private readonly IPasswordProvider _passwordProvider;
    private readonly DbContextOptions<Context> _options;

    private readonly RestClient _client = new("http://localhost:5000");
    private readonly RestClient _mailClient = new(Environment.GetEnvironmentVariable("SMTP_UI") ?? "http://localhost:8025");
    private readonly string _username = "test_user";
    private readonly string _password = "Test123!";
    private readonly string _email = "test@local";
    private string _twoFaCode = "";
    private string _newPassword = "";
    private string _token = "";

    public AuthSteps(IPasswordProvider passwordProvider)
    {
        _passwordProvider = passwordProvider;
        _options = DBHooks.DbOptions;
    }

    [Given(@"a technical user exists")]
    public async Task GivenATechnicalUserExists()
    {
        await using var ctx = new Context(_options);

        var existing = await ctx.Users
            .FirstOrDefaultAsync(x => x.Username == _username);

        if (existing != null)
        {
            ctx.Users.Remove(existing);
            await ctx.SaveChangesAsync();
        }

        var user = new UserDb(
            username: _username,
            password: _passwordProvider.HashPassword(_password),
            email: _email,
            type: UserType.Employee);

        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();
    }

    [When(@"I login with correct username and password")]
    public async Task WhenILoginWithCorrectUsernameAndPassword()
    {
        var req = new RestRequest("/api/v1/auth/2fa/login", Method.Post)
            .AddJsonBody(new LoginRequestDto(Username: _username, Password: _password));

        var resp = await _client.ExecuteAsync(req);
        Console.WriteLine($"Login response: IsSuccessful={resp.IsSuccessful}, Error={resp.ErrorMessage}");

        resp.IsSuccessful.Should().BeTrue();
    }

    [Then(@"a 2FA code is sent to my email")]
    public async Task Read2FaEmail()
    {
        //
        // 1. Получаем список писем
        //
        var listResp = await _mailClient.ExecuteAsync(new RestRequest("/api/messages"));
        listResp.IsSuccessful.Should().BeTrue();
        listResp.Content.Should().NotBeEmpty();

        using var jsonDoc = JsonDocument.Parse(listResp.Content);
        var results = jsonDoc.RootElement.GetProperty("results");
        results.GetArrayLength().Should().BeGreaterThan(0);

        //
        // 2. Берём последнее письмо по дате
        //
        var last = results.EnumerateArray()
            .OrderByDescending(x =>
                x.GetProperty("receivedDate").GetDateTime())
            .First();

        var id = last.GetProperty("id").GetString();

        //
        // 3. Получаем текст письма напрямую
        //
        var messageResp = await _mailClient.ExecuteAsync(
            new RestRequest($"/api/messages/{id}/plaintext")
        );

        messageResp.IsSuccessful.Should().BeTrue();
        messageResp.Content.Should().NotBeEmpty();
        
        var body = messageResp.Content;

        //
        // 4. Ищем код (6 цифр)
        //
        var match = Regex.Match(body, @"\b(\d{6})\b");
        match.Success.Should().BeTrue();

        _twoFaCode = match.Groups[1].Value;
    }

    [When(@"I confirm 2FA with the correct code")]
    public async Task ConfirmCorrect2Fa()
    {
        var req = new RestRequest("/api/v1/auth/2fa/confirm", Method.Post)
            .AddJsonBody(new TwoFactorConfirmRequestDto(Username: _username, Code: _twoFaCode));

        var resp = await _client.ExecuteAsync(req);

        resp.IsSuccessful.Should().BeTrue();
        resp.Content.Should().NotBeEmpty();

        _token = resp.Content;
    }

    [When(@"I confirm 2FA with wrong code three times")]
    public async Task ConfirmWrong3Times()
    {
        for (var i = 0; i < 3; i++)
        {
            var req = new RestRequest("/api/v1/auth/2fa/confirm", Method.Post)
                .AddJsonBody(new TwoFactorConfirmRequestDto(Username: _username, Code: "000000"));

            await _client.ExecuteAsync(req);
        }
    }

    [Then(@"my password is automatically changed")]
    public async Task CheckEmailForNewPassword()
    {
        //
        // 1. Получаем список писем
        //
        var listResp = await _mailClient.ExecuteAsync(new RestRequest("/api/messages"));
        listResp.IsSuccessful.Should().BeTrue();
        listResp.Content.Should().NotBeEmpty();
        
        using var jsonDoc = JsonDocument.Parse(listResp.Content);
        var results = jsonDoc.RootElement.GetProperty("results");
        results.GetArrayLength().Should().BeGreaterThan(0);

        //
        // 2. Находим последнее письмо (должно содержать новый пароль)
        //
        var last = results.EnumerateArray()
            .OrderByDescending(x =>
                x.GetProperty("receivedDate").GetDateTime())
            .First();

        var id = last.GetProperty("id").GetString();

        //
        // 3. Получаем текст письма
        //
        var messageResp = await _mailClient.ExecuteAsync(
            new RestRequest($"/api/messages/{id}/plaintext")
        );

        messageResp.IsSuccessful.Should().BeTrue();

        messageResp.Content.Should().NotBeEmpty();
        var body = messageResp.Content;

        //
        // 4. Извлекаем новый пароль
        //
        
        var match = Regex.Match(body, @"пароль:\s*(\S+)");
        match.Success.Should().BeTrue();
        
        _newPassword = match.Groups[1].Value.Trim('"');
    }

    [Then(@"I receive a new password via email")]
    public void ValidateNewPassword()
    {
        _newPassword.Should().NotBeNullOrEmpty();
    }

    [Given(@"I already triggered 2FA block")]
    public async Task GivenIAlreadyTriggeredFaBlock()
    {
        await WhenILoginWithCorrectUsernameAndPassword();
        await ConfirmWrong3Times();
        await CheckEmailForNewPassword();
    }

    [When(@"I login using the temporary password")]
    public async Task LoginWithTempPassword()
    {
        var req = new RestRequest("/api/v1/auth/2fa/login", Method.Post)
            .AddJsonBody(new LoginRequestDto(Username: _username, Password: _newPassword));

        var resp = await _client.ExecuteAsync(req);

        resp.IsSuccessful.Should().BeTrue();
    }

    [Then(@"a JWT token is returned")]
    public void TokenReturned()
    {
        _token.Should().Contain(".");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;

        _client.Dispose();
        _mailClient.Dispose();
    }
}