using Domain.Models;

namespace DataAccess.Models.Converters;

public static class PersonConverter
{
    public static Person ToDomain(this PersonDb person)
    {
        return new Person(person.Id, person.Sex, person.FullName, person.BirthDate);
    }
}