using System.ComponentModel;

namespace DTOs.Enum;

public enum RelationshipTypeDto
{
    [Description("Родитель")]
    Parent,
    [Description("Приёмный родитель")]
    StepParent,
    [Description("Ребёнок")]
    Child,
    [Description("Приёмный ребёнок")]
    StepChild,
    [Description("Друг")]
    Friend,
    [Description("Супруг(а)")]
    Spouse,
    [Description("Бывший супруг(а)")]
    ExSpouse,
    [Description("Брат/Сестра")]
    Sibling,
    [Description("Сводный брат/сестра")]
    StepSiblings,
    [Description("Работодатель")]
    Employer,
    [Description("Сотрудник")]
    Employee,
    [Description("Коллега")]
    Colleague,
    [Description("Наставник")]
    Mentor,
    [Description("Подопечный")]
    Protege,
    [Description("Бывшие отношения")]
    NoMore,
}