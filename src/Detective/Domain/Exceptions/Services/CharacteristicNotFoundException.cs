namespace Domain.Exceptions.Services;

public class CharacteristicNotFoundException : ServiceException
{

    public CharacteristicNotFoundException() : base("Characteristic not found")
    {
    }

    public CharacteristicNotFoundException(Guid id) : base($"Characteristic not found with Id {id}")
    {
    }
}