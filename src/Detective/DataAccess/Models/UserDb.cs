using Domain.Enum;

namespace DataAccess.Models;

public class UserDb(string username,
    string password,
    UserType type)
{
    public string Username = username;
    public string Password = password;
    public UserType Type = type;

    public ICollection<CharacteristicDb> Characteristics;
}