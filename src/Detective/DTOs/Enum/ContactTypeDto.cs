using System.ComponentModel;

namespace DTOs.Enum;

public enum ContactTypeDto
{
    [Description("Номер телефонда")]
    Phone,
    [Description("Почта")]
    Email,
    Telegram,
    Vk
}