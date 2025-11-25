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
        return relationship switch
        {
            RelationshipType.Parent => RelationshipType.Child,
            RelationshipType.StepParent => RelationshipType.StepChild,
            RelationshipType.Child => RelationshipType.Parent,
            RelationshipType.StepChild => RelationshipType.StepParent,
            RelationshipType.Friend => RelationshipType.Friend, // Дружба взаимна
            RelationshipType.Spouse => RelationshipType.Spouse, // Супружество взаимно
            RelationshipType.ExSpouse => RelationshipType.ExSpouse, // Бывшие супруги также взаимны
            RelationshipType.Sibling => RelationshipType.Sibling, // Родные братья/сестры взаимны
            RelationshipType.StepSiblings => RelationshipType.StepSiblings, // Сводные братья/сестры взаимны
            RelationshipType.Employer => RelationshipType.Employee,
            RelationshipType.Employee => RelationshipType.Employer,
            RelationshipType.Colleague => RelationshipType.Colleague,
            RelationshipType.Mentor => RelationshipType.Protege,
            RelationshipType.Protege => RelationshipType.Mentor,
            RelationshipType.NoMore => RelationshipType.NoMore,
            _ => throw new ArgumentException("Unknown relationship type: " + relationship)
        };
    }
}