using Domain.Enums;

namespace DataAccess.Models;

public class RelationshipDb(Guid person1Id,
    Guid person2Id,
    RelationshipType type)
{
    public Guid Person1Id { get; } = person1Id;
    public Guid Person2Id { get; } = person2Id;
    public RelationshipType Type { get; set; } = type;

    public virtual PersonDb Person1 { get; set; }

    public virtual PersonDb Person2 { get; set; }
}