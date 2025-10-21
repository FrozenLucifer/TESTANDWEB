
using DTOs.Enum;

namespace DTOs;

public class ContactDto
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public ContactTypeDto Type { get; set; }
    public required string Info { get; set; }
}