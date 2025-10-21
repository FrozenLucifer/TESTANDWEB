using DTOs.Enum;

namespace DTOs;

public class PersonFullDto
{
    public Guid Id { get; set; }
    public SexDto? Sex { get; set; }
    public string? FullName { get; set; }
    public DateOnly? BirthDate { get; set; }
    public List<ContactDto> Contacts { get; set; }
    public List<DocumentDto> Documents { get; set; }
    public List<PropertyDto> Properties { get; set; }
    public List<CharacteristicDto> Characteristics { get; set; }

    public PersonFullDto(
        Guid id,
        SexDto? sex,
        string? fullName,
        DateOnly? birthDate,
        List<ContactDto> contacts,
        List<DocumentDto> documents,
        List<PropertyDto> properties,
        List<CharacteristicDto> characteristics)
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