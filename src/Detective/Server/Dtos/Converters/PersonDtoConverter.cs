using Domain.Models;
using DTOs;

namespace Detective.Dtos.Converters;

public static class PersonDtoConverter
{
    public static PersonDto ToDto(this Person person)
    {
        return new PersonDto(person.Id, person.Sex.ToDto(), person.FullName, person.BirthDate);
    }
}