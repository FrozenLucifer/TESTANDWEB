using Domain.Enums;
using Domain.Models;
using Domain.Exceptions.Repositories;
using Domain.Exceptions.Services;

namespace Domain.Interfaces.Repository;

public interface IUserRepository
{
    /// <summary>
    /// Создать пользователя
    /// </summary>
    /// <param name="login">Логин пользователя</param>
    /// <param name="password">Пароль пользователя (хэш)</param>
    /// <param name="type">Тип пользователя (<see cref="UserType"/>)</param>
    /// <exception cref="UserAlreadyExistsException">Пользователь с таким логином уже существует</exception>
    public Task CreateUser(string login, string password, string email, UserType type);

    /// <summary>
    /// Получить пользователя по логину
    /// </summary>
    /// <param name="login">Логин пользователя</param>
    /// <returns>Пользователя</returns>
    /// <exception cref="UserNotFoundRepositoryException">Пользователя с таким логином не существует</exception>
    public Task<User> GetUser(string login);

    /// <summary>
    /// Поменять пароль пользователю
    /// </summary>
    /// <param name="login">Логин пользователя</param>
    /// <param name="password">Новый пароль пользователю (хэш)</param>
    /// <exception cref="UserNotFoundException">Пользователя с таким логином не существует</exception>
    public Task ChangePassword(string login, string password);

    /// <summary>
    /// Удалить пользователя по логину
    /// </summary>
    /// <param name="login">Логин пользователя</param>
    /// <exception cref="UserNotFoundException">Пользователя с таким логином не существует</exception>
    public Task DeleteUser(string login);

    public Task<List<User>> GetUsers();
}