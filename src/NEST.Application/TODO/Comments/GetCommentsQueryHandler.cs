using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;
using NEST.Domain.Entities.TODO;

namespace NEST.Application.TODO.Comments;

// Handler выполняет действие, описанное в GetCommentsQuery
public class GetCommentsQueryHandler
    : IRequestHandler<GetCommentsQuery, List<Comment>?>
{
    private readonly IApplicationDbContext _context;

    public GetCommentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Comment>?> Handle(
        GetCommentsQuery request,
        CancellationToken cancellationToken)
    {
        // Проверяем, существует ли задача
        var taskExists = await _context.Tasks
            .AnyAsync(
                t => t.Id == request.TaskId,
                cancellationToken);

        // Если задача не найдена - null
        if (!taskExists)
        {
            return null;
        }

        // Получаем комментарии задачи
        return await _context.Comments
            .Where(c => c.TaskId == request.TaskId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}