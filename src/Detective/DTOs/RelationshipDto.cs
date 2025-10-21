using DTOs.Enum;

namespace DTOs;

public class RelationshipDto
{
    public Guid Person1Id { get; set; }
    public Guid Person2Id { get; set; }
    public RelationshipTypeDto Type { get; set; }
}