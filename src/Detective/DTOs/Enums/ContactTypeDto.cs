using System.ComponentModel;

namespace DTOs.Enums;

public enum ContactTypeDto
{
    [Description("Номер телефонда")]
    Phone,
    [Description("Почта")]
    Email,
    Telegram,
    Vk
}