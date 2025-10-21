namespace Domain.Exceptions.Repositories;

public class PersonAlreadyExistsRepositoryException(Guid id) : RepositoryException($"Person already exists with Id {id}");