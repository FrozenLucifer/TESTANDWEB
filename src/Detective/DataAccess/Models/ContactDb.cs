using Domain.Enums;

namespace DataAccess.Models;

public class ContactDb(Guid id, Guid personId, ContactType type, string info)
{
    public Guid Id { get; set; }= id;
    public Guid PersonId { get; set; }= personId;
    public ContactType Type { get; set; }= type;
    public string Info { get; set; }= info;

    public virtual PersonDb Person { get; set; }
}