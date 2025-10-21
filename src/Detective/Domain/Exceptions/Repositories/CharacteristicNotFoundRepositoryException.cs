namespace Domain.Exceptions.Repositories;

public class CharacteristicNotFoundRepositoryException(Guid id) : RepositoryException($"Characteristic not found with Id {id}");