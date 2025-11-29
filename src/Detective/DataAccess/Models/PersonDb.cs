using Domain.Enums;

namespace DataAccess.Models;

public class PersonDb(Guid id,
    Sex? sex,
    string? fullName,
    DateOnly? birthDate)
{
    public Guid Id = id;
    public Sex? Sex = sex;
    public string? FullName = fullName;
    public DateOnly? BirthDate = birthDate;

    public ICollection<DocumentDb> Documents;
    public ICollection<ContactDb> Contacts;
    public ICollection<PropertyDb> Properties;
    public ICollection<RelationshipDb> RelationshipsAsPerson1;
    public ICollection<RelationshipDb> RelationshipsAsPerson2;
    public ICollection<CharacteristicDb> Characteristics;
}