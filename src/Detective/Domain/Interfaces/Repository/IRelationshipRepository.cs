using Domain.Enums;
using Domain.Models;
using Domain.Exceptions.Repositories;

namespace Domain.Interfaces.Repository;

public interface IRelationshipRepository
{
    /// <summary>
    /// Устанавливает отношения между людьми. Если отношения уже существуют, обновляет их. Если type = null, удаляет существующие отношения.
    /// </summary>
    /// <param name="person1Id"></param>
    /// <param name="person2Id"></param>
    /// <param name="type"></param>
    /// <exception cref="PersonNotFoundRepositoryException">Одного или обоих людей не существует.</exception>
    /// <exception cref="RelationshipNotFoundRepositoryException">type = null, но отношений не существует</exception>
    public Task SetRelationship(Guid person1Id, Guid person2Id, RelationshipType? type);

    public Task<RelationshipType> GetPersonsRelationship(Guid person1Id, Guid person2Id);

    public Task<List<Relationship>> GetPersonRelationships(Guid id);
}