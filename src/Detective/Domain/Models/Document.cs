using Domain.Enum;

namespace Domain.Models;

public class Document(Guid id, Guid personId, DocumentType type, string payload)
{
    public Guid Id = id;
    public Guid PersonId = personId;
    public DocumentType Type = type;
    public string Payload = payload;
}