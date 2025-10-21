namespace Domain.Exceptions.Services.Auth;

public class BadPasswordAuthException(string message) : AuthException($"Bad password: {message}");