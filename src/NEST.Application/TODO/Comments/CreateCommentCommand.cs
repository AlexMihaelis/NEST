using MediatR;

namespace NEST.Application.TODO.Comments;

// Команда на создание нового комментария. В ответ Handler вернет Guid созданного комментария
public record CreateCommentCommand(string Content, Guid TaskId, Guid UserId) : IRequest<Guid?>;