using Domain.Enum;

namespace Domain.Models;

public class Relationship(Guid personId1,
    Guid personId2,
    RelationshipType type)
{
    public Guid PersonId1 = personId1;
    public Guid PersonId2 = personId2;
    public RelationshipType Type = type;

    public Relationship Opposite()
    {
        return new Relationship(PersonId2, PersonId1, RelationshipHelper.GetInverseRelationship(Type));
    }

    public override string ToString()
    {
        return $"({PersonId1})->({PersonId2})[{Type}]";
    }

    public override bool Equals(object obj) => Equals(obj as Relationship);

    public bool Equals(Relationship? other)
    {
        if (other is null) return false;

        return (PersonId1 == other.PersonId1 &&
                PersonId2 == other.PersonId2 &&
                Type == other.Type);
    }
}