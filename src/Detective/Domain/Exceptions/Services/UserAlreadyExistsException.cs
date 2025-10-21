namespace Domain.Exceptions.Services;

public class UserAlreadyExistsException(string username) : ServiceException($"User already exists with username {username}");