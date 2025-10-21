namespace Domain.Exceptions.Repositories;

public class PropertyNotFoundRepositoryException(Guid id) : RepositoryException($"Property not found with Id {id}");