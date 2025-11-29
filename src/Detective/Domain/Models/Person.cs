using Domain.Enums;

namespace Domain.Models;

public class Person(Guid id, Sex? sex, string? fullName, DateOnly? birthDate)
{
    public Guid Id = id;
    public Sex? Sex = sex;
    public string? FullName = fullName;
    public DateOnly? BirthDate = birthDate;
}