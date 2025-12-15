using Domain.Enums;

namespace DataAccess.Models;

public class DocumentDb(Guid id,
    Guid personId,
    DocumentType type,
    string payload)
{
    public Guid Id { get; set; } = id;
    public Guid PersonId { get; set; } = personId;
    public DocumentType Type { get; set; } = type;
    public string Payload { get; set; } = payload;

    public virtual PersonDb Person { get; set; }
}