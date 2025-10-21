using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DTOs.Enum;

namespace DTOs;

/// <summary>
/// DTO для фильтрации списка людей
/// </summary>
public class GetPersonFilterDto
{
    /// <summary>
    /// Фильтр по полу
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SexDto? Sex { get; set; }

    /// <summary>
    /// Фильтр по имени/фамилии (поиск по частичному совпадению)
    /// </summary>
    [MaxLength(100)]
    public string? FullName { get; set; }

    /// <summary>
    /// Минимальная граница даты рождения (рожденные после указанной даты)
    /// </summary>
    public DateOnly? MinBirthDate { get; set; }

    /// <summary>
    /// Максимальная граница даты рождения (рожденные до указанной даты)
    /// </summary>
    public DateOnly? MaxBirthDate { get; set; }

    public ContactTypeDto? ContactType { get; set; }
    
    public string? ContactInfo { get; set; }
    
    /// <summary>
    /// Максимальное количество возвращаемых записей
    /// </summary>
    // [Range(1, 1000)]
    public int? Limit { get; set; } = null;

    public int? Skip { get; set; } = null;
}