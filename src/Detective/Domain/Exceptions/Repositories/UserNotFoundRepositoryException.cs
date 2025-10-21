namespace Domain.Exceptions.Repositories;

public class UserNotFoundRepositoryException(string username) : RepositoryException($"User not found with username {username}");