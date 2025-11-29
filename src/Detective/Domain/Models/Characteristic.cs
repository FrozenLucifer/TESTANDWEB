namespace Domain.Models;

public class Characteristic(Guid id,
    Guid personId,
    string authorUsername,
    string appearance,
    string personality,
    string medicalConditions)
{
    public Guid Id { get; }= id;
    public Guid PersonId { get; }= personId;
    public string AuthorUsername { get; }= authorUsername;

    public string Appearance { get; }= appearance;
    public string Personality { get; }= personality;
    public string MedicalConditions { get; }= medicalConditions;
}