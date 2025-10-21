using System.ComponentModel.DataAnnotations;
using Detective.Dtos;
using Detective.Dtos.Converters;
using Detective.Extensions;
using Domain.Enum;
using Domain.Interfaces.Service;
using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Detective.Controllers;

[ApiController]
[Route("api/v1/users")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Создать пользователя с автосгенерированным паролем 
    /// </summary>
    [HttpPost]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(string))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status409Conflict)]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = Policies.Admin)]
    public async Task<ActionResult<string>> CreateUser(CreateUserDto createUserDto)
    {
        var tmpPassword = await _userService.CreateUser(createUserDto.username, createUserDto.type);
        return Ok(tmpPassword);
    }


    /// <summary>
    /// Удалить пользователя
    /// </summary>
    [HttpDelete("{username}")]
    [SwaggerResponse(StatusCodes.Status204NoContent)]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = Policies.Admin)]
    public async Task<ActionResult> DeleteUser([Required] string username)
    {
        await _userService.DeleteUser(username);
        return NoContent();
    }

    /// <summary>
    /// Сбросить пароль 
    /// </summary>
    [HttpPut("{username}/password")]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(string))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = Policies.Admin)]
    public async Task<ActionResult<string>> ResetPassword([Required] string username)
    {
        var newPassword = await _userService.ResetPassword(username);
        return Ok(newPassword);
    }

    
    /// <summary>
    /// Получить пользователей
    /// </summary>
    [HttpGet]
    [SwaggerResponse(StatusCodes.Status200OK, type: typeof(List<UserDto>))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Нет доступа")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable)]
    [Authorize(Policy = Policies.Admin)]
    public async Task<ActionResult<List<UserDto>>> GetUsers()
    {
        var users = await _userService.GetUsers();
        return Ok(users.ConvertAll(UserDtoConverter.ToDto));
    }
}