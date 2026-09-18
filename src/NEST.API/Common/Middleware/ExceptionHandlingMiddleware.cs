using FluentValidation;
using System.Text.Json;

namespace NEST.API.Common.Middleware;

// Middleware - компонент, который получает HTTP-запрос и может выполнить код до и после передачи запроса следующему компоненту

// Перехватывает исключения из приложения и превращает их в корректные HTTP-ответы
public class ExceptionHandlingMiddleware
{
    // RequestDelegate - это делегат, который представляет следующий компонент Pipeline
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Передаем запрос дальше по Pipeline
            await _next(context);
        }
        catch (ValidationException exception)
        {
            // Ошибки валидации возвращаем как "400 Bad Request"
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errors = exception.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).ToArray());

            var response = new
            {
                message = "Validation failed",
                errors
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}