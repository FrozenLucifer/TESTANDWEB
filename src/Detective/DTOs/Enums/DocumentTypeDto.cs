using System.ComponentModel;

namespace DTOs.Enums;

public enum DocumentTypeDto
{
    [Description("Паспорт")]
    Passport,
    [Description("Поддельный паспорт")]
    FakePassport,
    [Description("Водительское удостоверение")]
    DriverLicense,
}