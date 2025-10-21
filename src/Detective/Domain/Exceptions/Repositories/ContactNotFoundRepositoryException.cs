namespace Domain.Exceptions.Repositories;

public class ContactNotFoundRepositoryException(Guid id) : RepositoryException($"Contact not found with Id {id}");