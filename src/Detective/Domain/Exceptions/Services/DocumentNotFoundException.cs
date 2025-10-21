namespace Domain.Exceptions.Services;

public class DocumentNotFoundException(Guid id) : ServiceException($"Document not found with Id {id}");