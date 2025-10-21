namespace Domain.Exceptions.Services;

public class ContactAlreadyExistsException(Guid id) : ServiceException($"Contact already exists with Id {id}");