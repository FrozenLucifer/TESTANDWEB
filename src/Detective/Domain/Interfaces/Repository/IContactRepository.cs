using Domain.Enum;
using Domain.Models;
using Domain.Exceptions.Repositories;

namespace Domain.Interfaces.Repository;

public interface IContactRepository
{
    /// <summary>
    /// Создать контакт пользователю
    /// </summary>
    /// <param name="id">Id контакта</param>
    /// <param name="personId">Id пользователя</param>
    /// <param name="type">Тип контакта</param>
    /// <param name="info">Информация о контакте</param>
    /// <exception cref="PersonNotFoundRepositoryException">Человека с таким Id не существует</exception>
    /// <exception cref="ContactAlreadyExistsRepositoryException">Контакт с таким Id уже существует</exception>
    public Task CreateContact(Guid id, Guid personId, ContactType type, string info);

    /// <summary>
    /// Получить контакты пользователя
    /// </summary>
    /// <param name="personId">Id пользователя</param>
    /// <param name="type">Тип контакта для фильтрации (опционально). Если null - возвращаются все контакты</param>
    /// <returns>Список контактов пользователя</returns>
    /// <exception cref="PersonNotFoundRepositoryException">Человека с таким Id не существует</exception>
    public Task<List<Contact>> GetPersonContacts(Guid personId, ContactType? type = null);

    /// <summary>
    /// Удалить контакт
    /// </summary>
    /// <param name="id">Id контакта</param>
    /// <exception cref="ContactNotFoundRepositoryException">Контакт с таким Id не найден</exception>
    public Task DeleteContact(Guid id);

    /// <summary>
    /// Получить Id пользователя по контактным данным
    /// </summary>
    /// <param name="contactType">Тип контакта</param>
    /// <param name="info">Информация о контакте</param>
    /// <returns>Id пользователя, которому принадлежит контакт</returns>
    /// <exception cref="PersonNotFoundRepositoryException">Пользователь с указанным контактом не найден</exception>
    public Task<Person> GetPersonByContact(ContactType contactType, string info);
}