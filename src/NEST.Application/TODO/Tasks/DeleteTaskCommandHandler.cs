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
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == request.TaskId, cancellationToken);
        
        if (task is null)
        {
            return false;
        }
        
        var columnId = task.ColumnId;
        var position = task.Position;
        
        _context.Tasks.Remove(task);
        
        // Сдвигаем позиции следующих задач наверх
        var tasksToShift = await _context.Tasks
            .Where(t => t.ColumnId == columnId && t.Position > position)
            .ToListAsync(cancellationToken);

        foreach (var taskToShift in tasksToShift)
        {
            taskToShift.Position--;
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}