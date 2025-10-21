namespace Domain.Exceptions.Repositories;

public class PersonNotFoundRepositoryException : RepositoryException
{

    public PersonNotFoundRepositoryException() : base("Person not found")
    {
    }

    public PersonNotFoundRepositoryException(Guid id) : base($"Person not found with Id {id}")
    {
    }
}