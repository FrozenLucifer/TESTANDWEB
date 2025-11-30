using Domain.Exceptions.Services.Auth;


namespace Domain.Interfaces.Service;

public interface IAuthService
{
    /// <summary>
    /// Авторизация пользователя
    /// </summary>
    /// <param name="username">Логин пользователя</param>
    /// <param name="password">Пароль пользователя (не хэш)</param>
    /// <returns>JWT-токен</returns>
    /// <exception cref="UserNotFoundAuthException">Пользователь не найден</exception>
    /// <exception cref="WrongPasswordAuthException">Неправильный пароль</exception>
    public Task<string> Login(string username, string password);

    /// <summary>
    /// Сменить пароль пользователя
    /// </summary>
    /// <param name="username">Логин пользователя</param>
    /// <param name="oldPassword">Старый пароль пользователя (не хэш)</param>
    /// <param name="newPassword">Новый пароль пользователя (не хэш)</param>
    /// <exception cref="UserNotFoundAuthException">Пользователь не найден</exception>
    /// <exception cref="WrongPasswordAuthException">Неправильный пароль</exception>
    /// <exception cref="BadPasswordAuthException">Плохой пароль</exception>
    public Task ChangePassword(string username, string oldPassword, string newPassword);
    
    Task TwoFactorLogin(string username, string password);

    Task<string> TwoFactorConfirm(string username, string code);
}