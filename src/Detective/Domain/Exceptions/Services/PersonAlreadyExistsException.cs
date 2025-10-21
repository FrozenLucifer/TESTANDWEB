namespace Domain.Exceptions.Services;

public class PersonAlreadyExistsException(Guid id) : ServiceException($"Person already exists with Id {id}");