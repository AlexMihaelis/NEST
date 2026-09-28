using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;

namespace NEST.Application.TODO.Comments;

// Handler выполняет действие, описанное в DeleteCommentCommand
public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteCommentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        DeleteCommentCommand request,
        CancellationToken cancellationToken)
    {
        // Ищем комментарий по Id
        var comment = await _context.Comments.FirstOrDefaultAsync(
            c => c.Id == request.CommentId,
            cancellationToken);

        // Если комментарий не найден - возвращаем false
        if (comment is null)
        {
            return false;
        }

        // Удаляем комментарий
        _context.Comments.Remove(comment);

        // Сохраняем изменения в БД
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}