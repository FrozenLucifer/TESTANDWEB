using System.Net;
using System.Net.Http.Json;
using DataAccess;
using DataAccess.Models;
using Detective;
using Domain.Enum;
using DTOs;
using FluentAssertions;
using Logic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Rest;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
    }
}

public class AuthE2ETests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly Context _db;

    public AuthE2ETests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        _scope = factory.Services.CreateScope();
        _db = _scope.ServiceProvider.GetRequiredService<Context>();
    }

    public Task InitializeAsync()
    {
        var username = "test_user";
        var passwordHash = new PasswordProvider().HashPassword("OldPassword123!");
        var user = new UserDb(username, passwordHash, "", UserType.Admin);

        _db.Users.Add(user);
        _db.SaveChangesAsync();

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _db.Users.RemoveRange(_db.Users);
        _db.SaveChangesAsync();
        return Task.CompletedTask;
    }

    // public void Dispose()
    // {
    //     _client.Dispose();
    //     _scope.Dispose();
    //     _factory.Dispose();
    // }

    [Fact]
    public async Task AuthAndChangePassword_FullFlow_WorksCorrectly()
    {
        var username = "test_user";
        var oldPassword = "OldPassword123!";
        var newPassword = "NewPassword456!";

        var wrongLoginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequestDto(
                Username: username,
                Password: "wrong_pass"
            ));

        wrongLoginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // 2️⃣ Успешная авторизация
        var correctLoginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequestDto(
                Username: username,
                Password: oldPassword
            ));

        correctLoginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginData = await correctLoginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
        loginData.Should().NotBeNull();
        loginData.Token.Should().NotBeNullOrWhiteSpace();

        // Добавляем токен к заголовкам
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginData.Token);

        // 3️⃣ Смена пароля
        var changePasswordResponse = await _client.PatchAsJsonAsync("/api/v1/auth/password",
            new ChangePasswordRequestDto(Username: username, OldPassword: oldPassword, NewPassword: newPassword));

        changePasswordResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 4️⃣ Проверяем вход со старым паролем
        var oldLoginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequestDto(Username: username, Password: oldPassword));

        oldLoginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // 5️⃣ Проверяем вход с новым паролем
        var newLoginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new LoginRequestDto(Username: username, Password: newPassword));

        newLoginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}