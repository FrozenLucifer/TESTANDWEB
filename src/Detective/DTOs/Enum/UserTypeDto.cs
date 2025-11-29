using System.Text.Json.Serialization;

namespace DTOs.Enum;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserTypeDto
{
    Employee,
    Admin,
    SpecialUser
}