using Domain.Models;

namespace DataAccess.Models.Converters;

public static class UserConverter
{
    public static User ToDomain(this UserDb user)
    {
        return new User(user.Type, user.Username, user.Password, user.Email);
    }
}