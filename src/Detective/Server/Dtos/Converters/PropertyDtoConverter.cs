using Domain.Models;
using DTOs;

namespace Detective.Dtos.Converters;

public static class PropertyDtoConverter
{
    public static PropertyDto ToDto(this Property property)
    {
        return new PropertyDto
        {
            Id = property.Id,
            PersonId = property.PersonId,
            Type = property.Type,
            Cost = property.Cost
        };
    }
}