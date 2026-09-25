using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;
using Task = NEST.Domain.Entities.TODO.Task;

namespace NEST.Application.TODO.Tasks;

public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, List<Task>?>
{
    private readonly IApplicationDbContext _context;
    
    public GetTasksQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Task>?> Handle(
        GetTasksQuery request,
        CancellationToken cancellationToken)
    {
        var columnExists = await _context.Columns
            .AnyAsync(c => c.Id == request.ColumnId, cancellationToken);
        
        if (!columnExists)
        {
            return null;
        }
        
        // Получаем задачи колонки и сортируем их попозиции
        return await _context.Tasks
            .Where(t => t.ColumnId == request.ColumnId)
            .OrderBy(t => t.Position)
            .ToListAsync(cancellationToken);
    }
}