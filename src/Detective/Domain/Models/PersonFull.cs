using Domain.Enums;

namespace Domain.Models;

public class PersonFull(
    Guid id,
    Sex? sex,
    string? fullName,
    DateOnly? birthDate,
    IReadOnlyCollection<Contact> contacts,
    IReadOnlyCollection<Document> documents,
    IReadOnlyCollection<PersonProperty> properties,
    IReadOnlyCollection<Characteristic> characteristics)
    : Person(id, sex, fullName, birthDate)
{
    public IReadOnlyCollection<Contact> Contacts { get; set; } = contacts;
    public IReadOnlyCollection<Document> Documents { get; set; } = documents;
    public IReadOnlyCollection<PersonProperty> Properties { get; set; } = properties;
    public IReadOnlyCollection<Characteristic> Characteristic { get; set; } = characteristics;

    public PersonFull(Person person,
        IReadOnlyCollection<Contact> contacts,
        IReadOnlyCollection<Document> documents,
        IReadOnlyCollection<PersonProperty> properties,
        IReadOnlyCollection<Characteristic> characteristics) :
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