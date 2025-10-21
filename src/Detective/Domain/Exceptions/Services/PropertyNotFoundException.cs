namespace Domain.Exceptions.Services;

public class PropertyNotFoundException(Guid id) : ServiceException($"Property not found with Id {id}");