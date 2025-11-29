namespace Domain.Models;

public class PassportPayload
{
    public required string Series { get; set; } // Серия (4 цифры)
    public required string Number { get; set; } // Номер (6 цифр)
    public string? IssuedBy { get; set; } // Кем выдан
    public DateTime? IssueDate { get; set; } // Дата выдачи
    public DateTime? BirthDate { get; set; } // Дата рождения
}