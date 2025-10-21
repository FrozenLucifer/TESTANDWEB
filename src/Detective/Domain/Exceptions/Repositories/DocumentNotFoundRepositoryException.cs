namespace Domain.Exceptions.Repositories;

public class DocumentNotFoundRepositoryException(Guid id) : RepositoryException($"Document not found with Id {id}");