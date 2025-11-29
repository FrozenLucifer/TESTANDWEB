using Domain.Enums;

namespace Domain.Models;

public class Relationship(Guid personId1,
    Guid personId2,
    RelationshipType type)
{
    public Guid PersonId1 { get; } = personId1;
    public Guid PersonId2 { get; } = personId2;
    public RelationshipType Type { get; } = type;

    public Relationship Opposite()
    {
        return new Relationship(PersonId2, PersonId1, RelationshipHelper.GetInverseRelationship(Type));
    }

    public override string ToString()
    {
        return $"({PersonId1})->({PersonId2})[{Type}]";
    }

    public override bool Equals(object? obj) => Equals(obj as Relationship);

    public override int GetHashCode()
    {
        return HashCode.Combine(PersonId1, PersonId2, (int)Type);
    }

    private bool Equals(Relationship? other)
    {
        if (other is null) return false;

        return (PersonId1 == other.PersonId1 &&
                PersonId2 == other.PersonId2 &&
                Type == other.Type);
    }
}