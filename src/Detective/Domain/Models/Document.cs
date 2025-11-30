using Domain.Enums;

namespace Domain.Models;

public class Document(Guid id, Guid personId, DocumentType type, string payload)
{
    public Guid Id { get; }= id;
    public Guid PersonId { get; }= personId;
    public DocumentType Type { get; }= type;
    public string Payload { get; }= payload;
}