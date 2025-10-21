namespace Domain.Exceptions.Repositories;

public class DocumentAlreadyExistsRepositoryException(Guid id) : RepositoryException($"Document already exists with Id {id}");