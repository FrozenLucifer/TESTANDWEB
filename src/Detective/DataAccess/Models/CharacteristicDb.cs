namespace DataAccess.Models;

public class CharacteristicDb(Guid id,
    Guid personId,
    string authorUsername,
    string appearance,
    string personality,
    string medicalConditions)
{
    public Guid Id { get; set; }= id;
    public Guid PersonId { get; set; }= personId;
    public string AuthorUsername { get; set; }= authorUsername;

    public string Appearance { get; set; }= appearance;
    public string Personality { get; set; }= personality;
    public string MedicalConditions { get; set; }= medicalConditions;

    public virtual PersonDb Person { get; set; }
    public virtual UserDb Author { get; set; }
}