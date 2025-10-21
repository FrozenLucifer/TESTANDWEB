using Domain.Enum;
using Domain.Models;
using Domain.Exceptions;
using Domain.Exceptions.Repositories;

namespace Domain.Interfaces.Repository;

public interface IRelationshipRepository
{
    /// <summary>
    /// Устанавливает отношения между людьми. Если отношения уже существуют, обновляет их. Если type = null, удаляет существующие отношения.
    /// </summary>
    /// <param name="id1"></param>
    /// <param name="id2"></param>
    /// <param name="type"></param>
    /// <exception cref="PersonNotFoundRepositoryException">Одного или обоих людей не существует.</exception>
    /// <exception cref="RelationshipNotFoundRepositoryException">type = null, но отношений не существует</exception>
    public Task SetRelationship(Guid id1, Guid id2, RelationshipType? type);

    public Task<RelationshipType> GetPersonsRelationship(Guid id1, Guid id2);

    public Task<List<Relationship>> GetPersonRelationships(Guid id);
}