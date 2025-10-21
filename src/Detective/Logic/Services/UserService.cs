using Domain.Enum;
using Domain.Exceptions;
using Domain.Exceptions.Repositories;
using Domain.Exceptions.Services;
using Domain.Interfaces;
using Domain.Interfaces.Repository;
using Domain.Interfaces.Service;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Logic.Services;

public class UserService(IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ILogger<UserService> logger) : IUserService
{
    public async Task<string> CreateUser(string username, UserType userType)
    {
        try
        {
            logger.LogInformation("Creating new user with username: {Username}", username);

            var password = GenerateTemporaryPassword();
            var passwordHash = passwordHasher.HashPassword(password);
            await userRepository.CreateUser(username, passwordHash, userType);

            logger.LogInformation("User {Username} created successfully", username);
            return password;
        }
        catch (UserAlreadyExistsRepositoryException ex)
        {
            logger.LogError(ex, "User {Username} already exists", username);
            throw new UserAlreadyExistsException(username);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task<string> ResetPassword(string username)
    {
        try
        {
            logger.LogInformation("Resetting password for user: {Username}", username);

            var password = GenerateTemporaryPassword();
            var passwordHash = passwordHasher.HashPassword(password);
            await userRepository.ChangePassword(username, passwordHash);

            logger.LogInformation("Password reset for user {Username} completed", username);
            return password;
        }
        catch (UserNotFoundException ex)
        {
            logger.LogError(ex, "User {Username} not found for password reset", username);
            throw new UserNotFoundException(username);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task DeleteUser(string username)
    {
        try
        {
            logger.LogInformation("Deleting user: {Username}", username);

            await userRepository.DeleteUser(username);

            logger.LogInformation("User {Username} deleted successfully", username);
        }
        catch (UserNotFoundRepositoryException ex)
        {
            logger.LogError(ex, "User {Username} not found for deletion", username);
            throw new UserNotFoundException(username);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unexpected error");
            throw;
        }
    }

    public async Task<List<User>> GetUsers()
    {
        var users = await userRepository.GetUsers();
        return users;
    }

    private string GenerateTemporaryPassword()
    {
        const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        var chars = new char[12];

        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = validChars[random.Next(validChars.Length)];
        }

        return new string(chars);
    }
}