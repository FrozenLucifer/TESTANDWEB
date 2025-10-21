using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Configurations;
using Domain.Exceptions;
using Domain.Exceptions.Repositories;
using Domain.Exceptions.Services.Auth;
using Domain.Interfaces;
using Domain.Interfaces.Repository;
using Domain.Interfaces.Service;
using Domain.Models;
using Logic.Validators;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Logic.Services;

public class AuthService(IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IOptions<JwtConfiguration> jwtOptions,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly JwtConfiguration _jwtConfiguration = jwtOptions.Value;
    private readonly PasswordValidator _passwordValidator = new PasswordValidator();

    public async Task<string> Login(string username, string password)
    {
        logger.LogInformation("Attempting login for user: {Username}", username);

        try
        {
            var user = await VerifyUserPassword(username, password);
            logger.LogDebug("User {Username} authenticated successfully", username);

            var token = GenerateJwtToken(user);
            logger.LogInformation("JWT token generated for user: {Username}", username);

            return token;
        }
        catch (UserNotFoundRepositoryException)
        {
            logger.LogWarning("User not found with username: {Username}", username);
            throw new UserNotFoundAuthException();
        }
        catch (WrongPasswordAuthException)
        {
            logger.LogWarning("Wrong password for user {Username}", username);
            throw new WrongPasswordAuthException();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error");
            throw;
        }
    }

    public async Task ChangePassword(string username, string oldPassword, string newPassword)
    {
        logger.LogInformation("Password change requested for user: {Username}", username);

        var validationResult = await _passwordValidator.ValidateAsync(newPassword);

        if (!validationResult.IsValid)
        {
            logger.LogWarning("Password validation failed for user {Username}. Errors: {Errors}",
                username,
                validationResult.GetErrorMessages());
            throw new BadPasswordAuthException(validationResult.GetErrorMessages());
        }

        try
        {
            await VerifyUserPassword(username, oldPassword);
            logger.LogDebug("Old password verified for user {Username}", username);

            var newPasswordHash = passwordHasher.HashPassword(newPassword);
            await userRepository.ChangePassword(username, newPasswordHash);

            logger.LogInformation("Password changed successfully for user {Username}", username);
        }
        catch (UserNotFoundRepositoryException)
        {
            logger.LogWarning("User not found: {Username}", username);
            throw new UserNotFoundAuthException();
        }
        catch (WrongPasswordAuthException)
        {
            logger.LogWarning("Wrong old password for user {Username}", username);
            throw new WrongPasswordAuthException();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error");
            throw;
        }
    }

    private async Task<User> VerifyUserPassword(string username, string password)
    {
        var user = await userRepository.GetUser(username);
        if (!passwordHasher.VerifyPassword(user.Password, password))
            throw new WrongPasswordAuthException();

        return user;
    }

    private string GenerateJwtToken(User user)
    {
        logger.LogDebug("Generating JWT token for user: {Username}", user.Username);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfiguration.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Type.ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.Add(_jwtConfiguration.TokenLifetime),
            signingCredentials: credentials
        );

        logger.LogDebug("JWT token generated successfully for user: {Username}", user.Username);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}