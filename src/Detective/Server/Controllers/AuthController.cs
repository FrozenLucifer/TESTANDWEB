using System.ComponentModel.DataAnnotations;
using Detective.Dtos;
using Domain.Interfaces.Service;
using DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Detective.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    /// <summary>
    /// Авторизация через логин-пароль
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    [SwaggerResponse(StatusCodes.Status200OK, "Аутентификация прошла успешно", type: typeof(LoginResponseDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Неверный логин или пароль")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Внутренний сервис не доступен")]
    public async Task<ActionResult<LoginResponseDto>> Auth([FromBody] LoginRequestDto request)
    {
        var result = await _authService.Login(request.Username, request.Password);
        var response = new LoginResponseDto(result);
        return Ok(response);
    }


    /// <summary>
    /// Сменить пароль
    /// </summary>
    [Authorize]
    [HttpPatch("password")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Пароль успешно изменен")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Новый пароль слабый")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Неверный логин или пароль")]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "Внутренний сервис не доступен")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        await _authService.ChangePassword(request.Username, request.OldPassword, request.NewPassword);
        return NoContent();
    }
}