using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DTOs.Enum;

namespace DTOs;

public class CreatePersonDto
{
    /// <summary>
    /// Фильтр по полу
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SexDto? Sex { get; set; }

    /// <summary>
    /// ФИО / часть ФИО
    /// </summary>
    [MaxLength(100)]
    public string? FullName { get; set; }

    /// <summary>
    /// Дата рождения
    /// </summary>
    public DateOnly? BirthDate { get; set; }
}