using Domain.Enum;

namespace Domain.Models;

public class Contact(Guid id, Guid personId, ContactType type, string info)
{
    public Guid Id = id;
    public Guid PersonId = personId;
    public ContactType Type = type;
    public string Info = info;
}