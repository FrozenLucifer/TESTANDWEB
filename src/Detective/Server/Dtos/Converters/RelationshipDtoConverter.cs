using Domain.Models;
using DTOs;

namespace Detective.Dtos.Converters;

public static class RelationshipDtoConverter
{
    public static RelationshipDto ToDto(this Relationship relationship)
    {
        return new RelationshipDto
        {
            Person1Id = relationship.PersonId1,
            Person2Id = relationship.PersonId2,
            Type = relationship.Type.ToDto(),
        };
    }
}