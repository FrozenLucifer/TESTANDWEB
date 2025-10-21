namespace Domain.Exceptions.Services;

public class PersonNotFoundException : ServiceException
{

    public PersonNotFoundException() : base("Person not found")
    {
    }

    public PersonNotFoundException(Guid id) : base($"Person not found with Id {id}")
    {
    }
}

public class PersonServiceException() : ServiceException("Person Service error");
