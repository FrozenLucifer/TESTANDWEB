using DataAccess.Models;
using DataAccess.Models.Converters;
using Domain.Enum;
using Domain.Exceptions;
using Domain.Exceptions.Repositories;
using Domain.Interfaces.Repository;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository;

public class UserRepository : IUserRepository
{
    private readonly Context _context;

    public UserRepository(Context context)
    {
        _context = context;
    }

    public async Task CreateUser(string login, string password, UserType type)
    {
        try
        {
            var userDb = new UserDb(login, password, type);

            _context.Users.Add(userDb);

            await _context.SaveChangesAsync();
        }
        catch (Exception ex) when (ex is DbUpdateException or InvalidOperationException)
        {
            throw new UserAlreadyExistsRepositoryException(login);
        }
    }

    public async Task<User> GetUser(string login)
    {
        var userDb = await _context.Users.FindAsync(login);
        if (userDb is null)
            throw new UserNotFoundRepositoryException(login);

        return userDb.ToDomain();
    }

    public async Task ChangePassword(string login, string password)
    {
        var userDb = await _context.Users.FindAsync(login);
        if (userDb is null)
            throw new UserNotFoundRepositoryException(login);

        userDb.Password = password;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUser(string login)
    {
        var userDb = await _context.Users.FindAsync(login);
        if (userDb is null)
            throw new UserNotFoundRepositoryException(login);

        _context.Users.Remove(userDb);
        await _context.SaveChangesAsync();
    }

    public async Task<List<User>> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        return users.ConvertAll(UserConverter.ToDomain);
    }
}