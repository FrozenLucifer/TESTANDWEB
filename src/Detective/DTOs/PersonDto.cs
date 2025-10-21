using DTOs.Enum;

namespace DTOs;

public class PersonDto
{
    public Guid Id { get; set; }
    public SexDto? Sex { get; set; }
    public string? FullName { get; set; }
    public DateOnly? BirthDate { get; set; }

    public PersonDto(Guid id, SexDto? sex, string? fullName, DateOnly? birthDate)
    {
        Id = id;
        Sex = sex;
        FullName = fullName;
        BirthDate = birthDate;
    }
}