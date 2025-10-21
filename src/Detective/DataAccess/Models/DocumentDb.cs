using Domain.Enum;

namespace DataAccess.Models;

public class DocumentDb(Guid id,
    Guid personId,
    DocumentType type,
    string payload)
{
    public Guid Id = id;
    public Guid PersonId = personId;
    public DocumentType Type = type;
    public string Payload = payload;

    public virtual PersonDb Person { get; set; }
}