namespace Domain.Exceptions.Repositories;

public class CharacteristicAlreadyExistsRepositoryException(Guid id) : RepositoryException($"Characteristic already exists with Id {id}");