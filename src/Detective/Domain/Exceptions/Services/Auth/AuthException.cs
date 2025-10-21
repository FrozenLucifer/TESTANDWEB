namespace Domain.Exceptions.Services.Auth;

public class AuthException : ServiceException
{
    public AuthException() : base("Auth exception")
    {
    }

    public AuthException(string message) : base($"Auth exception: {message}")
    {
    }
}