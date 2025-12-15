using System.Text.Json.Serialization;

namespace DTOs.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserTypeDto
{
    Employee,
    Admin,
    SpecialUser
}