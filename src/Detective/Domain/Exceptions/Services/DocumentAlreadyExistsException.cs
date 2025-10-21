namespace Domain.Exceptions.Services;

public class DocumentAlreadyExistsException(Guid id) : ServiceException($"Document already exists with Id {id}");