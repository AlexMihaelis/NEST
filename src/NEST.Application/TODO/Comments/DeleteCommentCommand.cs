using MediatR;

namespace NEST.Application.TODO.Comments;

// Команда на удаление существующего комментария. В ответ Handler вернет bool:
// true - если комментарий удален, false - если комментарий не найден
public record DeleteCommentCommand(Guid CommentId) : IRequest<bool>;