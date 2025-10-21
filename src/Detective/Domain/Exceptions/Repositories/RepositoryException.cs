using Domain.Exceptions.Services;

namespace Domain.Exceptions.Repositories;

public class RepositoryException(string message) : DetectiveException(message);