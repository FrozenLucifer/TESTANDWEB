namespace DataAccess.Models;

public class CharacteristicDb(Guid id,
    Guid personId,
    string authorUsername,
    string appearance,
    string personality,
    string medicalConditions)
{
    public Guid Id = id;
    public Guid PersonId = personId;
    public string AuthorUsername = authorUsername;

    public string Appearance = appearance;
    public string Personality = personality;
    public string MedicalConditions = medicalConditions;

    public virtual PersonDb Person { get; set; }
    public virtual UserDb Author { get; set; }
}