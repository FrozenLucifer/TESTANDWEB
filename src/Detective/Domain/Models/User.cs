using Domain.Enums;

namespace Domain.Models;

public class User(UserType type, string username, string password, string email)
{
    public UserType Type { get; }= type;
    public string Username { get; }= username;
    public string Password { get; }= password;
    public string Email { get; }= email;
}