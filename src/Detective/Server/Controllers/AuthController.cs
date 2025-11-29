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
        var token = await _authService.Login(request.Username, request.Password);
        var response = new LoginResponseDto(token);
        return Ok(response);
    }
    
    /// <summary>
    /// Запуск двухфакторной аутентификации: проверка логин/пароль + отправка кода
    /// </summary>
    [AllowAnonymous]
    [HttpPost("2fa/login")]
    [SwaggerResponse(StatusCodes.Status200OK, "Код отправлен на email")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Неверный логин или пароль")]
    public async Task<ActionResult> TwoFactorLogin([FromBody] LoginRequestDto request)
    {
        await _authService.TwoFactorLogin(request.Username, request.Password);

        return Ok();
    }


    /// <summary>
    /// Подтверждение двухфакторной аутентификации
    /// </summary>
    [AllowAnonymous]
    [HttpPost("2fa/confirm")]
    [SwaggerResponse(StatusCodes.Status200OK, "2FA подтверждена", typeof(LoginResponseDto))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Неверный или просроченный код")]
    public async Task<ActionResult> TwoFactorConfirm([FromBody] TwoFactorConfirmRequestDto request)
    {
        var token = await _authService.TwoFactorConfirm(request.Username, request.Code);
        var response = new LoginResponseDto(token);
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