using MediatR;

namespace NEST.Application.Users;

// Command (команда) - объект, который описывает действие, которое мы хотим выполнить

// Команда на создание нового пользователя
// В ответ Handler вернет Guid созданного пользователя
public class CreateUserCommand : IRequest<Guid>
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public DateOnly BirthDate { get; set; }
}