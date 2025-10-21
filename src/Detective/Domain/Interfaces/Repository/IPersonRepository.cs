using Domain.Enum;
using Domain.Models;
using Domain.Exceptions;
using Domain.Exceptions.Repositories;

namespace Domain.Interfaces.Repository;

public interface IPersonRepository
{
    /// <summary>
    /// Создает нового человека
    /// </summary>
    /// <param name="id">Уникальный идентификатор человека</param>
    /// <param name="sex">Пол человека</param>
    /// <param name="fullName">Полное имя человека</param>
    /// <param name="birthDate">Дата рождения человека</param>
    /// <returns>Созданный человек</returns>
    /// <exception cref="PersonAlreadyExistsRepositoryException">Человек с таким id уже существует</exception>
    public Task<Person> CreatePerson(Guid id, Sex? sex, string? fullName, DateOnly? birthDate);

    /// <summary>
    /// Получает список людей, соответствующих заданным критериям фильтрации
    /// </summary>
    /// <param name="sex">Пол для фильтрации (может быть null)</param>
    /// <param name="fullName">ФИО для фильтрации (может быть null)</param>
    /// <param name="minBirthDate">Минимальная дата рождения для фильтрации</param>
    /// <param name="maxBirthDate">Максимальная дата рождения для фильтрации</param>
    /// <param name="limit">Максимальное количество возвращаемых записей, если null - без ограничений</param>
    /// <returns>Люди</returns>
    public Task<List<Person>> GetPersons(Sex? sex, string? fullName, DateOnly? minBirthDate, DateOnly? maxBirthDate, int? limit, int? skip);

    /// <summary>
    /// Получает человека по уникальному идентификатору
    /// </summary>
    /// <param name="id">Уникальный идентификатор человека</param>
    /// <returns>Человек</returns>
    /// <exception cref="PersonNotFoundRepositoryException">Человек с таким идентификатором не существует</exception>
    public Task<Person> GetPerson(Guid id);

    /// <summary>
    /// Обновляет данные существующего человека
    /// </summary>
    /// <param name="id">Уникальный идентификатор обновляемого человека</param>
    /// <param name="sex">Новое значение пола</param>
    /// <param name="fullName">Новое значение ФИО</param>
    /// <param name="birthDate">Новая дата рождения</param>
    /// <exception cref="PersonNotFoundRepositoryException">Человек с таким идентификатором не существует</exception>
    public Task UpdatePerson(Guid id, Sex? sex, string? fullName, DateOnly? birthDate);

    /// <summary>
    /// Удаляет человека с указанным идентификатором
    /// </summary>
    /// <param name="id">Уникальный идентификатор удаляемого человека</param>
    /// <exception cref="PersonNotFoundRepositoryException">Человек с таким идентификатором не существует</exception>
    public Task DeletePerson(Guid id);
}