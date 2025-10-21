using DTOs.Enum;

namespace DTOs;

public class UserDto
{
    public string Username { get; set; }
    public UserTypeDto Type { get; set; }

    public UserDto(string username, UserTypeDto type)
    {
        Username = username;
        Type = type;
    }
}