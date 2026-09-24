using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;
using NEST.Domain.Enums;

namespace NEST.Application.TODO.Columns;

public class DeleteColumnCommandHandler : IRequestHandler<DeleteColumnCommand, bool>
{
    private readonly IApplicationDbContext _context;
    
    public DeleteColumnCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    
    // Обрабатываем команду удаления колонки
    public async Task<bool> Handle(
        DeleteColumnCommand request,
        CancellationToken cancellationToken)
    {
        var column = await _context.Columns
            .FirstOrDefaultAsync(c => c.Id == request.ColumnId, cancellationToken);
        
        if (column is null)
        {
            return false;
        }
        
        // Если задачи нужно перенести в другую колонку
        if (request.Mode == DeleteColumnMode.MoveTasks)
        {
            // Ищем целевую колонку
            var targetColumn = await _context.Columns
                .FirstOrDefaultAsync(c => c.Id == request.TargetColumnId, cancellationToken);
            
            if (targetColumn is null)
            {
                return false;
            }
            
            // Получаем задачи исходной колонки
            var tasks = await _context.Tasks
                .Where(t => t.ColumnId == column.Id)
                .ToListAsync(cancellationToken);
            
            // Переносим задачи в целевую колонку
            foreach (var task in tasks)
            {
                task.ColumnId = targetColumn.Id;
            }
        }
        _context.Columns.Remove(column);
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}