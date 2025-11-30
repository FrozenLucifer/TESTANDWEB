using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Domain.Configurations;
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
    IPasswordProvider passwordProvider,
    ITwoFactorRepository twoFactorRepository,
    IEmailService emailService,
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
        catch (UserNotFoundRepositoryException ex)
        {
            logger.LogWarning(ex, "User not found with username: {Username}", username);
            throw new UserNotFoundAuthException();
        }
        catch (WrongPasswordAuthException ex)
        {
            logger.LogWarning(ex, "Wrong password for user {Username}", username);
            throw new WrongPasswordAuthException();
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

            var newPasswordHash = passwordProvider.HashPassword(newPassword);
            await userRepository.ChangePassword(username, newPasswordHash);

            logger.LogInformation("Password changed successfully for user {Username}", username);
        }
        catch (UserNotFoundRepositoryException ex)
        {
            logger.LogWarning(ex, "User not found: {Username}", username);
            throw new UserNotFoundAuthException();
        }
        catch (WrongPasswordAuthException ex)
        {
            logger.LogWarning(ex, "Wrong old password for user {Username}", username);
            throw new WrongPasswordAuthException();
        }
    }

    public async Task TwoFactorLogin(string username, string password)
    {
        logger.LogInformation("2FA login attempt for user: {Username}", username);

        var user = await VerifyUserPassword(username, password);

        var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString(CultureInfo.CurrentCulture);

        await twoFactorRepository.CreateCode(username, code);

        await emailService.SendAsync(
            user.Email,
            "Код подтверждения входа",
            $"Ваш код: {code}"
        );

        logger.LogInformation("2FA code sent to user: {Username}", username);
    }

    public async Task<string> TwoFactorConfirm(string username, string code)
    {
        logger.LogInformation("2FA confirm attempt for userId: {Username}", username);

        var storedCode = await twoFactorRepository.FindOrDefaultCode(username);

        if (storedCode == null)
        {
            logger.LogWarning("No 2FA code found for {Username}", username);
            throw new Wrong2FaCodeAuthException();
        }

        if (storedCode.FailedAttempts >= 3)
        {
            logger.LogWarning("User blocked {Username}", username);
            throw new Wrong2FaCodeAuthException();
        }

        if (storedCode.Code != code || storedCode.ExpiresAt < DateTime.UtcNow)
        {
            logger.LogWarning("Invalid or expired 2FA code for {Username}", username);

            storedCode.FailedAttempts++;
            
            await twoFactorRepository.UpdateFailedAttempts(username, storedCode.FailedAttempts);

            if (storedCode.FailedAttempts >= 3)
            {
                logger.LogWarning("Changing password for {Username}", username);
                var newPassword = passwordProvider.GenerateTemporaryPassword();
                var newPasswordHash = passwordProvider.HashPassword(newPassword);
                await userRepository.ChangePassword(username, newPasswordHash);

                var user1 = await userRepository.GetUser(username);

                await emailService.SendAsync(
                    user1.Email,
                    "Новый пароль",
                    $"В связи с подозрительной активностью, высылаем вам новый пароль: {newPassword}"
                );
            }

            throw new Wrong2FaCodeAuthException();
        }

        var user = await userRepository.GetUser(username);

        await twoFactorRepository.DeleteCode(username);

        var token = GenerateJwtToken(user);

        logger.LogInformation("2FA confirmed, token issued for {Username}", username);

        return token;
    }
    

    /// <summary>
    /// 
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref="WrongPasswordAuthException"></exception>
    /// <exception cref="UserNotFoundRepositoryException"></exception>
    private async Task<User> VerifyUserPassword(string username, string password)
    {
        var user = await userRepository.GetUser(username);
        if (!passwordProvider.VerifyPassword(user.Password, password))
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