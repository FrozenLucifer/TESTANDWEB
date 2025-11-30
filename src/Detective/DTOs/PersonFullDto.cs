using DTOs.Enum;

namespace DTOs;

public class PersonFullDto
{
    public Guid Id { get; set; }
    public SexDto? Sex { get; set; }
    public string? FullName { get; set; }
    public DateOnly? BirthDate { get; set; }
    public IReadOnlyCollection<ContactDto> Contacts { get; set; }
    public IReadOnlyCollection<DocumentDto> Documents { get; set; }
    public IReadOnlyCollection<PropertyDto> Properties { get; set; }
    public IReadOnlyCollection<CharacteristicDto> Characteristics { get; set; }

    public PersonFullDto(Guid id,
        SexDto? sex,
        string? fullName,
        DateOnly? birthDate,
        IReadOnlyCollection<ContactDto> contacts,
        IReadOnlyCollection<DocumentDto> documents,
        IReadOnlyCollection<PropertyDto> properties,
        IReadOnlyCollection<CharacteristicDto> characteristics)
    {
        Id = id;
        Sex = sex;
        FullName = fullName;
        BirthDate = birthDate;
        Contacts = contacts;
        Documents = documents;
        Properties = properties;
        Characteristics = characteristics;
    }
}