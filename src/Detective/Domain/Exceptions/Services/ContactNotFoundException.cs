namespace Domain.Exceptions.Services;

public class ContactNotFoundException(Guid id) : ServiceException($"Contact not found with Id {id}");