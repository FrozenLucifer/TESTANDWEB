namespace Domain.Exceptions.Services;

public class ServiceException(string message) : DetectiveException(message);