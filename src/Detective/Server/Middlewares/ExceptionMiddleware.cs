using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Exceptions.Services;
using Domain.Exceptions.Services.Auth;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Detective.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;
        var errorResponse = new ErrorResponse(exception);

        switch (exception)
        {
            case ValidationException:
                response.StatusCode = StatusCodes.Status400BadRequest;
                break;

            case UserNotFoundException:
            case PersonNotFoundException:
            case ContactNotFoundException:
            case DocumentNotFoundException:
            case PropertyNotFoundException:
            case RelationshipNotFoundException:
                errorResponse.Message = "Сущность не найдена";
                response.StatusCode = StatusCodes.Status404NotFound;
                break;

            case ContactAlreadyExistsException:
            case DocumentAlreadyExistsException:
            case PersonAlreadyExistsException:
            case PropertyAlreadyExistsException:
                errorResponse.Message = "Сущность уже существует";
                response.StatusCode = StatusCodes.Status409Conflict;
                break;

            case WrongPasswordAuthException:
            case UserNotFoundAuthException:
                errorResponse.Message = "Неправильный логин или пароль";
                response.StatusCode = StatusCodes.Status401Unauthorized;
                break;

            default:
                response.StatusCode = StatusCodes.Status500InternalServerError;
                errorResponse.Message = "Внутренняя ошибка сервера";
                _logger.LogError(exception, "Unhandled exception");
                break;
        }

        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}

public class ErrorResponse
{
    public string Message { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? StackTrace { get; set; }


    public ErrorResponse(Exception ex)
    {
        Message = ex.Message;
    }
}