using Domain.Enum;
using Domain.Exceptions;
using Domain.Exceptions.Services;
using Domain.Models;

namespace Domain.Interfaces.Service;

public interface IPersonService
{
    /// <summary>
    /// Получить пользователей по фильтрам
    /// </summary>
    /// <param name="sex">Фильтр по полу</param>
    /// <param name="fullName">Фильтр имени</param>
    /// <param name="maxBirthDate">Минимальная граница дня рождения</param>
    /// <param name="minBirthDate">Максимальная граница дня рождения</param>
    /// <param name="limit">Максимальное количество возвращаемых людей</param>
    /// <returns>Список людей, соответствующих критериям поиска</returns>
    public Task<List<Person>> GetPersons(Sex? sex, string? fullName, DateOnly? minBirthDate, DateOnly? maxBirthDate, int? limit = null, int? skip = null);

    /// <summary>
    /// Получить пользователя по Id
    /// </summary>
    /// <param name="id">Id пользователя</param>
    /// <returns>Полная информация о пользователе (контакты, документы, собственность)</returns>
    /// <exception cref="PersonNotFoundException">Пользователь с таким id не существует</exception>
    public Task<PersonFull> GetPerson(Guid id);

    /// <summary>
    /// Удалить пользователя по Id
    /// </summary>
    /// <param name="id">Id пользователя</param>
    /// <exception cref="PersonNotFoundException">Пользователь с таким id не существует</exception>
    public Task DeletePerson(Guid id);

    /// <summary>
    /// Создать нового пользователя
    /// </summary>
    /// <param name="sex">Пол пользователя</param>
    /// <param name="fullName">Полное имя пользователя</param>
    /// <param name="birthDate">Дата рождения пользователя</param>
    /// <returns>Id созданного пользователя</returns>
    public Task<Guid> CreatePerson(Sex? sex, string? fullName, DateOnly? birthDate);

    /// <summary>
    /// Изменить основную информацию о пользователе
    /// </summary>
    /// <param name="id">Id пользователя</param>
    /// <param name="sex">Новый пол пользователя</param>
    /// <param name="fullName">Новое полное имя пользователя</param>
    /// <param name="birthDate">Новая дата рождения пользователя</param>
    /// <exception cref="PersonNotFoundException">Пользователь с таким id не существует</exception>
    public Task ChangePersonGeneralInfo(Guid id, Sex? sex, string? fullName, DateOnly? birthDate);

    /// <summary>
    /// Получить связи пользователя
    /// </summary>
    /// <param name="personId">Id пользователя</param>
    /// <param name="depth">Глубина поиска связей (по умолчанию 1)</param>
    /// <param name="allowedTypes">Разрешенные типы связей(<see cref="RelationshipFilter"/>)</param>
    /// <returns>Список связей пользователя</returns>
    /// <exception cref="PersonNotFoundException">Пользователь с таким id не существует</exception>
    public Task<List<Relationship>> GetPersonRelationships(Guid personId, uint depth = 1, List<RelationshipType>? allowedTypes = null);

    /// <summary>
    /// Установить связь между людьми
    /// </summary>
    /// <param name="id1">Id первого пользователя</param>
    /// <param name="id2">Id второго пользователя</param>
    /// <param name="type">Тип устанавливаемой связи</param>
    /// <exception cref="PersonNotFoundException">Один из пользователей с указанными id не существует</exception>
    public Task SetPersonsRelationship(Guid id1, Guid id2, RelationshipType type);

    
    /// <summary>
    /// Удалить связь между людьми
    /// </summary>
    /// <param name="id1">Id первого пользователя</param>
    /// <param name="id2">Id второго пользователя</param>
    /// <exception cref="PersonNotFoundException">Один из пользователей с указанными id не существует</exception>
    public Task DeleteRelationship(Guid id1, Guid id2);

    /// <summary>
    /// Добавить документ пользователю
    /// </summary>
    /// <param name="personId">Id пользователя</param>
    /// <param name="type">Тип документа</param>
    /// <param name="info">Информация о документе</param>
    /// <exception cref="PersonNotFoundException">Пользователь с таким id не существует</exception>
    public Task AddPersonDocument(Guid personId, DocumentType type, string info);

    /// <summary>
    /// Добавить контакт пользователю
    /// </summary>
    /// <param name="personId">Id пользователя</param>
    /// <param name="type">Тип контакта</param>
    /// <param name="info">Контактная информация</param>
    /// <exception cref="PersonNotFoundException">Пользователь с таким id не существует</exception>
    public Task AddPersonContact(Guid personId, ContactType type, string info);

    /// <summary>
    /// Добавить собственность пользователю
    /// </summary>
    /// <param name="personId">Id пользователя</param>
    /// <param name="name">Название собственности</param>
    /// <param name="cost">Стоимость собственности</param>
    /// <exception cref="PersonNotFoundException">Пользователь с таким id не существует</exception>
    public Task AddPersonProperty(Guid personId, string name, int? cost);

    /// <summary>
    /// Удалить контакт пользователя
    /// </summary>
    /// <param name="contactId">Id контакта</param>
    /// <exception cref="ContactNotFoundException">Контакт с таким id не существует</exception>
    public Task DeletePersonContact(Guid contactId);

    /// <summary>
    /// Удалить документ пользователя
    /// </summary>
    /// <param name="documentId">Id документа</param>
    /// <exception cref="DocumentNotFoundException">Документ с таким id не существует</exception>
    public Task DeletePersonDocument(Guid documentId);

    /// <summary>
    /// Удалить собственность пользователя
    /// </summary>
    /// <param name="propertyId">Id собственности</param>
    /// <exception cref="PropertyNotFoundException">Собственность с таким id не существует</exception>
    public Task DeletePersonProperty(Guid propertyId);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="contactType"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    public Task<Person> GetPersonByContact(ContactType contactType, string info);


    public Task<Characteristic> GetCharacteristicById(Guid id);
    public Task<IEnumerable<Characteristic>> GetPersonsCharacteristics(Guid personId);

    public Task<Guid> CreateCharacteristic(Guid personId,
        string username,
        string appearance,
        string personality,
        string medicalConditions);

    public Task DeleteCharacteristic(Guid id);
}