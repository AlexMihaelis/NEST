using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;

namespace NEST.Application.TODO.Comments;

// Handler выполняет действие, описанное в UpdateCommentCommand
public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand, bool>
{
    private readonly IApplicationDbContext _context;
    
    public UpdateCommentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        // Ищем комментарий по Id
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == request.CommentId,
                cancellationToken);
        
        // Если комментарий не найден - false
        if (comment is null)
        {
            return false;
        }
        
        // Обновляем текст комментария
        comment.Content  = request.Content;
        comment.UpdatedAt = DateTime.UtcNow;
        
        // Сохраняем изменения в бд
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}