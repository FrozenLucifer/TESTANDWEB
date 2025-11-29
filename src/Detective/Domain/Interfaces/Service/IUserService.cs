using Domain.Enums;
using Domain.Exceptions.Services;
using Domain.Models;

namespace Domain.Interfaces.Service;

public interface IUserService
{
    /// <summary>
    /// Создать пользователя
    /// </summary>
    /// <param name="username">Логин пользователя</param>
    /// <param name="userType">Тип пользователя <see cref="UserType"/></param>
    /// <returns>Сгенерированный пароль пользователя</returns>
    /// <exception cref="UserAlreadyExistsException">Пользователь с таким логином уже существует</exception>
    public Task<string> CreateUser(string username, UserType userType);

    /// <summary>
    /// Сбросить пароль пользователя
    /// </summary>
    /// <param name="username">Логин пользователя</param>
    /// <returns>Сгенерированный пароль пользователя</returns>
    /// <exception cref="UserNotFoundException">Пользователя с таким логином не существует</exception>
    public Task<string> ResetPassword(string username);

    /// <summary>
    /// Удалить пользователя по логину
    /// </summary>
    /// <param name="username">Логин пользователя</param>
    /// <exception cref="UserNotFoundException">Пользователя с таким логином не существует</exception>
    public Task DeleteUser(string username);

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Task<List<User>> GetUsers();
}