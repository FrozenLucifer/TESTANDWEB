using Domain.Models;
using DTOs;

namespace Detective.Dtos.Converters;

public static class PersonFullDtoConverter
{
    public static PersonFullDto ToDto(this PersonFull person)
    {
        return new PersonFullDto(
            person.Id,
            person.Sex.ToDto(),
            person.FullName,
            person.BirthDate,
            person.Contacts.Select(c => c.ToDto()).ToList(),
            person.Documents.Select(d => d.ToDto()).ToList(),
            person.Properties.Select(p => p.ToDto()).ToList(),
            person.Characteristic.Select(p => p.ToDto()).ToList());
    }
}