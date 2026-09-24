using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;

namespace NEST.Application.TODO.Tasks;

// Handler выполняет действие, описанное 
public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, bool>
{
    private readonly IApplicationDbContext _context;
    
    public UpdateTaskCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        UpdateTaskCommand request,
        CancellationToken cancellationToken)
    {
        // Ищем задачу по Id
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);
        
        // Если задача не найдена - сообщаем
        if (task is null)
        {
            return false;
        }
        
        // Если указан контекст, проверяем, существует ли он
        if (request.TaskContextId.HasValue)
        {
            var taskContextExists = await _context.TaskContexts
                .AnyAsync(tc => tc.Id == request.TaskContextId.Value, cancellationToken);
            
            // Если контекст не найден - сообщаем
            if (!taskContextExists)
            {
                return false;
            }
        }
        
        // Обновляем данные задачи
        task.Name = request.Name;
        task.Description = request.Description;
        task.Priority = request.Priority;
        task.Deadline = request.Deadline;
        task.IsCompleted = request.IsCompleted;
        task.TaskContextId = request.TaskContextId;
        task.UpdatedAt = DateTime.UtcNow;
        
        // Сохраняем изменения в бд
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}