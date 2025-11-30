using System.Security.Cryptography;
using Domain.Enums;
using Domain.Exceptions.Repositories;
using Domain.Exceptions.Services;
using Domain.Interfaces;
using Domain.Interfaces.Repository;
using Domain.Interfaces.Service;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Logic.Services;

public class UserService(IUserRepository userRepository,
    IPasswordProvider passwordProvider,
    ILogger<UserService> logger) : IUserService
{
    public async Task<string> CreateUser(string username, UserType userType)
    {
        try
        {
            logger.LogInformation("Creating new user with username: {Username}", username);

            var password = passwordProvider.GenerateTemporaryPassword();
            var passwordHash = passwordProvider.HashPassword(password);
            await userRepository.CreateUser(username, passwordHash, "tmp@tmp.tmp", userType); //TODO

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

            var password = passwordProvider.GenerateTemporaryPassword();
            var passwordHash = passwordProvider.HashPassword(password);
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
}