using MediatR;
using NEST.Domain.Entities.TODO;

namespace NEST.Application.TODO.Comments;

// Query (запрос) для получения всех комментариев задачи
public record GetCommentsQuery(Guid TaskId) : IRequest<List<Comment>?>;