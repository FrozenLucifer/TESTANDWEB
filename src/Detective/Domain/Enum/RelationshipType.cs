namespace Domain.Enum;

public enum RelationshipType
{
    Parent,
    StepParent,
    Child,
    StepChild,
    Friend,
    Spouse,
    ExSpouse,
    Sibling,
    StepSiblings,
    Employer,
    Employee,
    Colleague,
    Mentor,
    Protege,
    NoMore,
}

public static class RelationshipFilter
{
    public static List<RelationshipType> Family() => new()
    {
        RelationshipType.Parent,
        RelationshipType.StepParent,
        RelationshipType.Child,
        RelationshipType.StepChild,
        RelationshipType.Spouse,
        RelationshipType.ExSpouse,
        RelationshipType.Sibling,
        RelationshipType.StepSiblings
    };

    public static List<RelationshipType> Work() => new()
    {
        RelationshipType.Employer,
        RelationshipType.Employee,
        RelationshipType.Colleague,
        RelationshipType.Mentor,
        RelationshipType.Protege
    };
}

public static class RelationshipHelper
{
    public static RelationshipType GetInverseRelationship(RelationshipType relationship)
    {
        switch (relationship)
        {
            case RelationshipType.Parent:
                return RelationshipType.Child;
            case RelationshipType.StepParent:
                return RelationshipType.StepChild;
            case RelationshipType.Child:
                return RelationshipType.Parent;
            case RelationshipType.StepChild:
                return RelationshipType.StepParent;
            case RelationshipType.Friend:
                return RelationshipType.Friend; // Дружба взаимна
            case RelationshipType.Spouse:
                return RelationshipType.Spouse; // Супружество взаимно
            case RelationshipType.ExSpouse:
                return RelationshipType.ExSpouse; // Бывшие супруги также взаимны
            case RelationshipType.Sibling:
                return RelationshipType.Sibling; // Родные братья/сестры взаимны
            case RelationshipType.StepSiblings:
                return RelationshipType.StepSiblings; // Сводные братья/сестры взаимны
            case RelationshipType.Employer:
                return RelationshipType.Employee;
            case RelationshipType.Employee:
                return RelationshipType.Employer;
            case RelationshipType.Colleague:
                return RelationshipType.Colleague;
            case RelationshipType.Mentor:
                return RelationshipType.Protege;
            case RelationshipType.Protege:
                return RelationshipType.Mentor;
            case RelationshipType.NoMore:
                return RelationshipType.NoMore;
            default:
                throw new ArgumentException("Unknown relationship type: " + relationship);
        }
    }
}