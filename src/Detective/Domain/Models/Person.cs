using Domain.Enums;

namespace Domain.Models;

public class Person(Guid id, Sex? sex, string? fullName, DateOnly? birthDate)
{
    public Guid Id { get; }= id;
    public Sex? Sex { get; }= sex;
    public string? FullName { get; }= fullName;
    public DateOnly? BirthDate { get; }= birthDate;
}