namespace Domain.Exceptions.Services.Auth;

public class WrongPasswordAuthException() : AuthException("Wrong password");

public class Wrong2FaCodeAuthException() : AuthException("Неверный или просроченный код подтверждения");