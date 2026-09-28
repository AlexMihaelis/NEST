using MediatR;

namespace NEST.Application.TODO.Comments;

// Команда на обновление существующего комментария.
// В ответ Handler вернет bool: true - если комментарий обновлен, false - если комментарий не найден
public record UpdateCommentCommand(Guid CommentId, string Content) : IRequest<bool>;