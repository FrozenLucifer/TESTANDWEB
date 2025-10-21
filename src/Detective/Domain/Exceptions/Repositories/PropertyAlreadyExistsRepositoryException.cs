namespace Domain.Exceptions.Repositories;

public class PropertyAlreadyExistsRepositoryException(Guid id) : RepositoryException($"Property already exists with Id {id}");