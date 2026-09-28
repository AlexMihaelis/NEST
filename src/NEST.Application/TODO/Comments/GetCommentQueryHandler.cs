using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;
using NEST.Domain.Entities.TODO;

namespace NEST.Application.TODO.Comments;

// Handler выполняет действие, описанное в GetCommentQuery
public class GetCommentQueryHandler
    : IRequestHandler<GetCommentQuery, Comment?>
{
    private readonly IApplicationDbContext _context;

    public GetCommentQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Comment?> Handle(
        GetCommentQuery request,
        CancellationToken cancellationToken)
    {
        // Ищем комментарий по Id
        // Если комментарий не найден - возвращаем null
        return await _context.Comments
            .FirstOrDefaultAsync(
                c => c.Id == request.CommentId,
                cancellationToken);
    }
}