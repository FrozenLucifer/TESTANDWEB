namespace Domain.Exceptions.Repositories;

public class UserAlreadyExistsRepositoryException(string username) : RepositoryException($"User already exists with username {username}");