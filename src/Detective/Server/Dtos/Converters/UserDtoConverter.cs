using Domain.Models;
using DTOs;

namespace Detective.Dtos.Converters;

public static class UserDtoConverter
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto(user.Username, user.Type.ToDto());
    }
}