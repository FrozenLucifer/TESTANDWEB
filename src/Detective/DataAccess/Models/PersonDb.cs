using Domain.Enums;

namespace DataAccess.Models;

public class PersonDb(Guid id,
    Sex? sex,
    string? fullName,
    DateOnly? birthDate)
{
    public Guid Id { get; set; } = id;
    public Sex? Sex { get; set; } = sex;
    public string? FullName { get; set; } = fullName;
    public DateOnly? BirthDate { get; set; } = birthDate;

    public ICollection<DocumentDb> Documents { get; set; }
    public ICollection<ContactDb> Contacts { get; set; }
    public ICollection<PropertyDb> Properties { get; set; }
    public ICollection<RelationshipDb> RelationshipsAsPerson1 { get; set; }
    public ICollection<RelationshipDb> RelationshipsAsPerson2 { get; set; }
    public ICollection<CharacteristicDb> Characteristics { get; set; }
}