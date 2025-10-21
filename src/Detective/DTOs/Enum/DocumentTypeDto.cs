using System.ComponentModel;

namespace DTOs.Enum;

public enum DocumentTypeDto
{
    [Description("Паспорт")]
    Passport,
    [Description("Поддельный паспорт")]
    FakePassport,
    [Description("Водительское удостоверение")]
    DriverLicense,
}