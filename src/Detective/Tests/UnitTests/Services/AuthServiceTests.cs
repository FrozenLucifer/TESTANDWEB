using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Domain.Configurations;
using Domain.Enum;
using Domain.Exceptions;
using Domain.Exceptions.Repositories;
using Domain.Exceptions.Services.Auth;
using Domain.Interfaces;
using Domain.Interfaces.Repository;
using Domain.Models;
using Logic.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IOptions<JwtConfiguration>> _jwtOptionsMock = new();
    private readonly AuthService _authService;

    private readonly JwtConfiguration _jwtConfig = new()
    {
        Key = "1234567890-1234567890-1234567890-1234567890",
        TokenLifetime = TimeSpan.FromHours(1)
    };

    public AuthServiceTests()
    {
        _jwtOptionsMock.Setup(x => x.Value).Returns(_jwtConfig);
        _authService = new AuthService(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtOptionsMock.Object,
            NullLogger<AuthService>.Instance);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsJwtToken()
    {
        // Arrange
        var username = "testUser";
        var password = "testPass";
        var type = UserType.Employee;
        var user = new User(username: username, password: "hashedPass", type: type);

        _userRepositoryMock.Setup(x => x.GetUser(username)).ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.VerifyPassword(user.Password, password)).Returns(true);

        // Act
        var result = await _authService.Login(username, password);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(result);

        Assert.Equal(username, token.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value);
        Assert.Equal(type.ToString(), token.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        Assert.True(token.ValidTo > DateTime.UtcNow);

        _userRepositoryMock.Verify(x => x.GetUser(username), Times.Once);
        _passwordHasherMock.Verify(x => x.VerifyPassword(user.Password, password), Times.Once);
    }

    [Fact]
    public async Task Login_UserNotFound_ThrowsUserNotFoundException()
    {
        // Arrange
        var username = "nonExistingUser";

        _userRepositoryMock.Setup(x => x.GetUser(username))
            .Throws(new UserNotFoundRepositoryException(username));

        // Act & Assert
        await Assert.ThrowsAsync<UserNotFoundAuthException>(() => _authService.Login(username, "anyPassword"));

        _userRepositoryMock.Verify(x => x.GetUser(username), Times.Once);
        _passwordHasherMock.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Login_WrongPassword_ThrowsWrongPasswordException()
    {
        // Arrange
        var username = "testUser";
        var password = "wrongPass";
        var user = new User(type: UserType.Employee, username: username, password: "hashedPass");

        _userRepositoryMock.Setup(x => x.GetUser(username)).ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.VerifyPassword(user.Password, password)).Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<WrongPasswordAuthException>(() => _authService.Login(username, password));

        _userRepositoryMock.Verify(x => x.GetUser(username), Times.Once);
        _passwordHasherMock.Verify(x => x.VerifyPassword(user.Password, password), Times.Once);
    }

    [Fact]
    public async Task ChangePassword_ValidInput_ChangesPassword()
    {
        // Arrange
        var username = "testUser";
        var oldPassword = "oldPass";
        var newPassword = "newValidPass123!";
        var user = new User(UserType.Employee, username, "hashedOldPass");

        _userRepositoryMock.Setup(x => x.GetUser(username)).ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.VerifyPassword(user.Password, oldPassword)).Returns(true);
        _passwordHasherMock.Setup(x => x.HashPassword(newPassword)).Returns("hashedNewPass");

        // Act
        await _authService.ChangePassword(username, oldPassword, newPassword);

        // Assert
        _userRepositoryMock.Verify(x => x.GetUser(username), Times.Once);
        _passwordHasherMock.Verify(x => x.VerifyPassword(user.Password, oldPassword), Times.Once);
        _passwordHasherMock.Verify(x => x.HashPassword(newPassword), Times.Once);
        _userRepositoryMock.Verify(x => x.ChangePassword(username, "hashedNewPass"), Times.Once);
    }

    [Fact]
    public async Task ChangePassword_UserNotFound_ThrowsUserNotFoundException()
    {
        // Arrange
        var username = "nonExistingUser";

        _userRepositoryMock.Setup(x => x.GetUser(username))
            .Throws(new UserNotFoundRepositoryException(username));

        // Act & Assert
        await Assert.ThrowsAsync<UserNotFoundAuthException>(() =>
            _authService.ChangePassword(username, "oldPass", "newPass1234567890"));

        _userRepositoryMock.Verify(x => x.GetUser(username), Times.Once);
        _passwordHasherMock.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _userRepositoryMock.Verify(x => x.ChangePassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ChangePassword_WrongOldPassword_ThrowsWrongPasswordException()
    {
        // Arrange
        var username = "testUser";
        var oldPassword = "wrongOldPass";
        var user = new User(UserType.Employee, username, "hashedOldPass");


        _userRepositoryMock.Setup(x => x.GetUser(username)).ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.VerifyPassword(user.Password, oldPassword)).Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<WrongPasswordAuthException>(() =>
            _authService.ChangePassword(username, oldPassword, "newPass123456789"));

        _userRepositoryMock.Verify(x => x.GetUser(username), Times.Once);
        _passwordHasherMock.Verify(x => x.VerifyPassword(user.Password, oldPassword), Times.Once);
        _userRepositoryMock.Verify(x => x.ChangePassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ChangePassword_InvalidNewPassword_ThrowsBadPasswordException()
    {
        // Arrange
        var username = "testUser";
        var oldPassword = "oldPass";
        var weakNewPassword = "123";

        var user = new User(UserType.Employee, username, "hashedOldPass");

        _userRepositoryMock.Setup(x => x.GetUser(username)).ReturnsAsync(user);
        _passwordHasherMock.Setup(x => x.VerifyPassword(user.Password, oldPassword)).Returns(true);

        // Act & Assert
        await Assert.ThrowsAsync<BadPasswordAuthException>(() =>
            _authService.ChangePassword(username, oldPassword, weakNewPassword));

        _userRepositoryMock.Verify(x => x.GetUser(username), Times.Never);
        _passwordHasherMock.Verify(x => x.VerifyPassword(user.Password, oldPassword), Times.Never);
        _passwordHasherMock.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        _userRepositoryMock.Verify(x => x.ChangePassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}