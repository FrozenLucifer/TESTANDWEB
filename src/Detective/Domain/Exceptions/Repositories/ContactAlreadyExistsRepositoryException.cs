namespace Domain.Exceptions.Repositories;

public class ContactAlreadyExistsRepositoryException(Guid id) : RepositoryException($"Contact already exists with Id {id}");