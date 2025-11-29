using Domain.Models;
using DTOs;

namespace Detective.Dtos.Converters;

public static class PropertyDtoConverter
{
    public static PropertyDto ToDto(this PersonProperty personProperty)
    {
        return new PropertyDto
        {
            Id = personProperty.Id,
            PersonId = personProperty.PersonId,
            Type = personProperty.Type,
            Cost = personProperty.Cost
        };
    }
}