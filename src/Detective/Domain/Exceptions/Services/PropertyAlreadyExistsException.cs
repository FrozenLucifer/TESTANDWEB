namespace Domain.Exceptions.Services;

public class PropertyAlreadyExistsException(Guid id) : ServiceException($"Property already exists with Id {id}");