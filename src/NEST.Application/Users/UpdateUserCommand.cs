using MediatR;

namespace NEST.Application.Users;

// Command (команда) - объект, который описывает действие, которое мы хотим выполнить

// Команда на обновление существующего пользователя
// В ответ Handler возвращает bool: true — пользователь обновлён, false — пользователь не найден
public class UpdateUserCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public DateOnly BirthDate { get; set; }
}