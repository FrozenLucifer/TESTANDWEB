using DTOs.Enum;

namespace DTOs;

public class DocumentDto
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public DocumentTypeDto Type { get; set; }
    public required string Payload { get; set; }
}