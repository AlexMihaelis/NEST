using MediatR;
using NEST.Domain.Entities;

namespace NEST.Application.Users;

// Query (запрос) - объект, который описывает получение данных

// Запрос на получение пользователя по Id
// В ответ Handler вернет данные пользователя
public class GetUserByIdQuery : IRequest<User?>
{
    // Id пользователя, которого нужно найти
    public Guid Id { get; set; }
}