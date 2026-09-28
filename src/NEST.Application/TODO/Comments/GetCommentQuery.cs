using MediatR;
using NEST.Domain.Entities.TODO;

namespace NEST.Application.TODO.Comments;

// Query (запрос) для получения одного комментария
public record GetCommentQuery(Guid CommentId) : IRequest<Comment?>;