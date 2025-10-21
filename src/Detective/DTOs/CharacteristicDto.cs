namespace DTOs;

public class CharacteristicDto
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public required string AuthorUsername { get; set; }
    public required string Appearance { get; set; }
    public required string Personality { get; set; }
    public required string MedicalConditions { get; set; }
}