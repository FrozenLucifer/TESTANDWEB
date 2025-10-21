namespace DTOs;

public class PropertyDto
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public required string Type { get; set; }
    public int? Cost { get; set; }
}