using Domain.Models;
using DTOs;

namespace Detective.Dtos.Converters;

public static class CharacteristicDtoConverter
{
    public static CharacteristicDto ToDto(this Characteristic characteristic)
    {
        return new CharacteristicDto
        {
            Id = characteristic.Id,
            PersonId = characteristic.PersonId,
            AuthorUsername = characteristic.AuthorUsername,
            Appearance = characteristic.Appearance,
            Personality = characteristic.Personality,
            MedicalConditions = characteristic.MedicalConditions
        };
    }
}