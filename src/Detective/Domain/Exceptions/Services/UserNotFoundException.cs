namespace Domain.Exceptions.Services;

public class UserNotFoundException(string username) : ServiceException($"User not found with username {username}");