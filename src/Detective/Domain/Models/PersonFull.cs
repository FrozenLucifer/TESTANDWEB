using Domain.Enum;

namespace Domain.Models;

public class PersonFull(
    Guid id,
    Sex? sex,
    string? fullName,
    DateOnly? birthDate,
    List<Contact> contacts,
    List<Document> documents,
    List<Property> properties,
    List<Characteristic> characteristics)
    : Person(id, sex, fullName, birthDate)
{
    public List<Contact> Contacts = contacts;
    public List<Document> Documents = documents;
    public List<Property> Properties = properties;
    public List<Characteristic> Characteristic = characteristics;

    public PersonFull(Person person,
        List<Contact> contacts,
        List<Document> documents,
        List<Property> properties,
        List<Characteristic> characteristics) :
        this(person.Id,
            person.Sex,
            person.FullName,
            person.BirthDate,
            contacts,
            documents,
            properties,
            characteristics)
    {
    }
}