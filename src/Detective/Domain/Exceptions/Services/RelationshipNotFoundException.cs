namespace Domain.Exceptions.Services;

public class RelationshipNotFoundException(Guid id1,
    Guid id2) : ServiceException($"Relationship not found between {id1} and {id2}");