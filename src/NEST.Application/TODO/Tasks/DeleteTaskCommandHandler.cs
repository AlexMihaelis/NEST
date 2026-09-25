using MediatR;
using Microsoft.EntityFrameworkCore;
using NEST.Application.Common.Interfaces;
using Task = NEST.Domain.Entities.TODO.Task;

namespace NEST.Application.TODO.Tasks;

// Handler выполняет действие, описанное в DeleteTaskCommand
public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteTaskCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        DeleteTaskCommand request,
        CancellationToken cancellationToken)
    {
        // Ищем задачу по Id
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);
        
        // Если задача не найдена - false
        if (task is null)
        {
            return false;
        }
        
        // Запоминаем колонку и позицию удаляемой задачи
        var columnId = task.ColumnId;
        var position = task.Position;
        
        // Удаляем задачу
        _context.Tasks.Remove(task);
        
        // Сдвигаем позиции следующих задач наверх
        var tasksToShift = await _context.Tasks
            .Where(t => t.ColumnId == columnId && t.Position > position)
            .ToListAsync(cancellationToken);

        foreach (var taskToShift in tasksToShift)
        {
            taskToShift.Position--;
        }
        
        // Сохраняем изменения в бд
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}