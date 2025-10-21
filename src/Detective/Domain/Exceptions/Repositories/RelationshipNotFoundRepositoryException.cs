namespace Domain.Exceptions.Repositories;

public class RelationshipNotFoundRepositoryException(Guid id1,
    Guid id2) : RepositoryException($"Relationship not found between {id1} and {id2}");