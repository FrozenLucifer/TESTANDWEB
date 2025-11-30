using Domain.Enums;

namespace Domain.Models;

public class Contact(Guid id, Guid personId, ContactType type, string info)
{
    public Guid Id { get; } = id;
    public Guid PersonId { get; }= personId;
    public ContactType Type { get; }= type;
    public string Info { get; }= info;
}