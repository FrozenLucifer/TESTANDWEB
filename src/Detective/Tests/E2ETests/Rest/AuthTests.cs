using System.Net;
using System.Net.Http.Json;
using DataAccess;
using DataAccess.Models;
using Domain.Enums;
using DTOs;
using FluentAssertions;
using Logic;
using Microsoft.EntityFrameworkCore;

namespace E2ETests.Rest;

public class AuthE2ETests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Context _db;
    private string _username;

    public AuthE2ETests()
    {
        var baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5000";
        _client = new HttpClient { BaseAddress = new Uri(baseUrl) };

        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                               ?? "Host=localhost;Port=5434;Database=detective_test;Username=postgres;Password=1";

        var options = new DbContextOptionsBuilder<Context>()
            .UseNpgsql(connectionString)
            .Options;

        _db = new Context(options);
    }

    public async Task InitializeAsync()
    {
        _username =  $"test_user{DateTime.Now.ToString()}";
        var passwordHash = new PasswordProvider().HashPassword("OldPassword123!");
        var user = new UserDb(_username, passwordHash, "", UserType.Admin);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        _db.Users.RemoveRange(_db.Users);
        await _db.SaveChangesAsync();
        await _db.DisposeAsync();
        _client.Dispose();
    }

    [Fact]
    public async Task AuthAndChangePassword_FullFlow_WorksCorrectly()
    {
        var username = _username;
        var oldPassword = "OldPassword123!";
        var newPassword = "NewPassword456!";

        var wrongLoginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequestDto(Username: username, Password: "wrong_pass"));

        wrongLoginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var correctLoginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequestDto(Username: username, Password: oldPassword));

        correctLoginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginData = await correctLoginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
        loginData.Should().NotBeNull();
        loginData.Token.Should().NotBeNullOrWhiteSpace();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginData.Token);

        var changePasswordResponse = await _client.PatchAsJsonAsync("/api/v1/auth/password",
            new ChangePasswordRequestDto(Username: username, OldPassword: oldPassword, NewPassword: newPassword));

        changePasswordResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var oldLoginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequestDto(Username: username, Password: oldPassword));

        oldLoginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var newLoginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequestDto(Username: username, Password: newPassword));

        newLoginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}