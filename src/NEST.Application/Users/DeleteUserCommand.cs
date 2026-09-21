using MediatR;

namespace NEST.Application.Users;

// Command (команда) - объект, который описывает действие, которое мы хотим выполнить

// Команда на удаление существующего пользовател. В ответ Handler возвращает bool: true — пользователь удалён, false — пользователь не найден
public class DeleteUserCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}