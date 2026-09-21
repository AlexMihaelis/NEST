using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
        catch (DbUpdateException exception)
        {
            // Получаем внутреннюю ошибку PostgreSQL
            if (exception.InnerException is PostgresException postgresException)
            {
                // Обрабатываем попытку создать пользователя с уже существующим Email или UserName
                if (postgresException.SqlState == "23505")
                {
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    context.Response.ContentType = "application/json";

                    var message = postgresException.ConstraintName switch
                    {
                        "IX_Users_Email" =>
                            "A user with this email already exists.",

                        "IX_Users_UserName" =>
                            "A user with this username already exists.",

                        _ =>
                            "A resource with the same unique value already exists."
                    };

                    var response = new
                    {
                        message
                    };

                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(response));

                    return;
                }

                // Обрабатываем попытку удалить пользователя, у которого есть связанные данные (например, доски)
                if (postgresException.ConstraintName is
                    "FK_Boards_Users_UserId" or
                    "FK_Comments_Users_UserId" or
                    "FK_Attachments_Users_UploadedByUserId")
                {
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    context.Response.ContentType = "application/json";

                    var response = new
                    {
                        message = "The user cannot be deleted because it has related data."
                    };

                    await context.Response.WriteAsync(
                        JsonSerializer.Serialize(response));

                    return;
                }
            }

            // Если это другая ошибка бд, которую мы пока не умеем обрабатывать, передаем исключение дальше
            throw;
        }
    }
}