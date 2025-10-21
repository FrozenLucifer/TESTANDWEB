using Domain.Enum;

namespace Domain.Models;

public class User(UserType type, string username, string password)
{
    public UserType Type = type;
    public string Username = username;
    public string Password = password;
}