using MediatR;
using NEST.Domain.Entities;

namespace NEST.Application.Users;

// Query (запрос) - объект, который описывает получение данных

// Запрос на получение всех пользователей. В ответ Handler вернет список пользователей
public class GetUsersQuery : IRequest<List<User>>
{
    
}